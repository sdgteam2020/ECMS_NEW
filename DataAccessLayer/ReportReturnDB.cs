using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Constants;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.Extensions.Logging;
using System.Data;

namespace DataAccessLayer
{
    public class ReportReturnDB : IReportReturnDB
    {
        private readonly DapperContext _contextDP;
        private readonly ILogger<ReportReturnDB> _logger;
        public ReportReturnDB(DapperContext contextDP, ILogger<ReportReturnDB> logger)
        {
            _contextDP = contextDP;
            _logger = logger;
        }


        /// <summary>
        /// Retrieves the Mstep count based on the provided hierarchy and ApplyForId.
        /// </summary>
        /// <param name="Data">The data that contains the hierarchy request parameters (e.g., ComdId, CorpsId, DivId, BdeId, etc.).</param>
        /// <param name="ApplyForId">The ApplyForId used to filter the MStepCount data.</param>
        /// <returns>A list of DTOReportReturnCount objects that represent the MStep count.</returns>
        /// <exception cref="Exception">Throws exception if there is an error during the database operation.</exception>
        public async Task<List<DTOReportReturnCount>> GetMstepCount(DTOMHierarchyRequest Data, int ApplyForId)
        {
            var (db, transaction) = _contextDP.CreateConnectionWithTransaction();
            try
            {
                string query = @" 
                    BEGIN TRY
                        BEGIN TRANSACTION;
                        
                        -- Creating temporary tables
                        CREATE TABLE #tempTrnFwds (
                            StepId INT,
                            IsComplete BIT,
                            TrnFwdId INT
                        );

                        INSERT INTO #tempTrnFwds (StepId, IsComplete, TrnFwdId)
                        SELECT fwd.StepId,fwd.IsComplete,fwd.TrnFwdId  FROM TrnFwds fwd
                        INNER JOIN TrnStepCounter step ON fwd.RequestId = step.RequestId AND fwd.StepId = step.StepId AND step.ApplyForId = @ApplyForId
                        LEFT JOIN TrnICardRequest req ON step.RequestId = req.RequestId AND req.StatusId = 1  
                        LEFT JOIN BasicDetails basi ON req.BasicDetailId = basi.BasicDetailId  
                        LEFT JOIN MapUnit unit ON basi.UnitId = unit.UnitMapId 
                        WHERE 
                            (
                                (@UnitType = 1 AND
                                    unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                    AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                    AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                    AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                )
                                OR
                                (@UnitType = 2 AND
                                    unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                    AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                    AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                    AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                    AND unit.FmnBranchID = ISNULL(@FmnBranchID, unit.FmnBranchID)
                                )
                                OR
                                (@UnitType = 3 AND
                                    unit.PsoId = ISNULL(@PsoId, unit.PsoId)
                                    AND unit.SubDteId = ISNULL(@SubDteId, unit.SubDteId)
                                )
                            )
                            AND unit.UnitMapId = ISNULL(@UnitMapId, unit.UnitMapId)
                            AND unit.UnitType =@UnitType
                        GROUP BY fwd.StepId, fwd.IsComplete, fwd.TrnFwdId;

                        CREATE TABLE #tempStep (
                            StepId INT,
                            IsDashboard BIT
                        );

                        INSERT INTO #tempStep (StepId, IsDashboard)
                        SELECT StepId, IsDashboard 
                        FROM MStepCounterStep 
                        WHERE IsDashboard = 1;

                        CREATE TABLE #Temp (
                            StepId INT,
                            IsApprove INT,
                            Total INT
                        );

                        DECLARE @Total INT = 0;
                        DECLARE @IsApprove INT = 0;
                        DECLARE @StepId INT = 0;

                        WHILE (SELECT COUNT(*) FROM #tempStep) > 0
                        BEGIN
                            SET @Total = 0;
                            SET @IsApprove = 0;
                            SET @StepId = 0;

                            SELECT TOP 1 @StepId = StepId FROM #tempStep;

                            IF EXISTS (SELECT 1 FROM #tempStep WHERE StepId = @StepId AND @StepId = 1)
                            BEGIN
                                SELECT @Total = COUNT(*) 
                                FROM TrnStepCounter step
                                LEFT JOIN TrnICardRequest req ON step.RequestId = req.RequestId AND req.StatusId = 1  
                                LEFT JOIN BasicDetails basi ON req.BasicDetailId = basi.BasicDetailId  
								LEFT JOIN MapUnit unit ON basi.UnitId = unit.UnitMapId 
                                WHERE 
                                    (
                                        (@UnitType = 1 AND
                                            unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                            AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                            AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                            AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                        )
                                        OR
                                        (@UnitType = 2 AND
                                            unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                            AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                            AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                            AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                            AND unit.FmnBranchID = ISNULL(@FmnBranchID, unit.FmnBranchID)
                                        )
                                        OR
                                        (@UnitType = 3 AND
                                            unit.PsoId = ISNULL(@PsoId, unit.PsoId)
                                            AND unit.SubDteId = ISNULL(@SubDteId, unit.SubDteId)
                                        )
                                    )
                                    AND unit.UnitMapId = ISNULL(@UnitMapId, unit.UnitMapId)
                                    AND unit.UnitType =@UnitType
                                    AND step.ApplyForId = @ApplyForId 
                                    AND step.StepId = @StepId;

                                INSERT INTO #Temp (StepId, IsApprove, Total)
                                VALUES (@StepId, @IsApprove, @Total);
                            END
                            ELSE IF EXISTS (SELECT 1 FROM #tempStep WHERE StepId = @StepId AND @StepId IN (2,3,4))
                            BEGIN
                                SELECT @Total = COUNT(*) 
                                FROM #tempTrnFwds 
                                WHERE StepId = @StepId AND IsComplete = 1;

                                INSERT INTO #Temp (StepId, IsApprove, Total)
                                VALUES (@StepId, 1, @Total);

                                SELECT @Total = COUNT(*) 
                                FROM #tempTrnFwds 
                                WHERE StepId = @StepId AND IsComplete = 0;

                                INSERT INTO #Temp (StepId, IsApprove, Total)
                                VALUES (@StepId, 0, @Total);
                            END
                            ELSE
                            BEGIN
                                INSERT INTO #Temp (StepId, IsApprove, Total)
                                VALUES (@StepId, 0, (SELECT COUNT(*) FROM #tempTrnFwds WHERE StepId = @StepId AND IsComplete = 0));
                            END

                            DELETE FROM #tempStep WHERE StepId = @StepId;
                        END

                        -- Final selection
                        SELECT 
                            a.StepId, 
                            a.IsApprove, 
                            a.Total, 
                            b.Name, 
                            b.TypeId, 
                            b.OrderBy  
                        FROM #Temp a 
                        INNER JOIN MStepCounterStep b 
                            ON a.StepId = b.StepId
                        ORDER BY b.TypeId, a.StepId, b.OrderBy;

                        COMMIT TRANSACTION;
                    END TRY
                    BEGIN CATCH
                        ROLLBACK TRANSACTION;

                        -- Drop temp tables if they exist
                        IF OBJECT_ID('tempdb..#tempTrnFwds') IS NOT NULL DROP TABLE #tempTrnFwds;
                        IF OBJECT_ID('tempdb..#tempStep') IS NOT NULL DROP TABLE #tempStep;
                        IF OBJECT_ID('tempdb..#Temp') IS NOT NULL DROP TABLE #Temp;

                        THROW;
                    END CATCH";
                var parameters = new DynamicParameters();
                parameters.Add("@ApplyForId", ApplyForId, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@ComdId", Data.ComdId, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@CorpsId", Data.CorpsId, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@DivId", Data.DivId, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@BdeId", Data.BdeId, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@FmnBranchID", Data.FmnBranchID, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@PsoId", Data.PsoId, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@SubDteId", Data.SubDteId, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@UnitMapId", Data.UnitMapId, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@UnitType", Data.UnitType, DbType.Int32, ParameterDirection.Input);

                var ret = await db.QueryAsync<DTOReportReturnCount>(query, parameters, transaction: transaction);
                // Commit the transaction if all operations succeed
                transaction.Commit();
                return ret.ToList();
            }
            catch (Exception ex)
            {
                // Rollback the transaction if any operation fails
                transaction.Rollback();
                _logger.LogError(1001, ex, "ReportReturnDB->GetMstepCount");
                return new List<DTOReportReturnCount>();
            }
            finally
            {
                // Dispose of the connection
                db.Dispose();
            }
        }


        /// <summary>
        /// Retrieves a list of Record Office Offers based on ArmedId.
        /// </summary>
        /// <param name="ArmedIdForORO">The ArmedId used to filter record office offers.</param>
        /// <returns>Returns a list of DTOReportReturnCount representing Record Office offers.</returns>
        public async Task<List<DTOReportReturnCount>> GetRecordOffOffers(short ArmedIdForORO)
        {
            string query = "select RecordOfficeId,Name from MRecordOffice where ArmedId=@ArmedIdForORO";
            try 
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTOReportReturnCount>(query,new { ArmedIdForORO });
                    return ret.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "ReportReturnDB->GetRecordJco");
                return new List<DTOReportReturnCount>();
            }
        }


        /// <summary>
        /// Retrieves the count of record office offers with various filtering based on Data.
        /// </summary>
        /// <param name="Data">The input data for hierarchical request.</param>
        /// <returns>Returns a list of DTOReportReturnCount representing counts of record office offers.</returns>
        public async Task<List<DTOReportReturnCount>> GetRecordOffOffersCount(DTOMHierarchyRequest Data)
        {

            string query = @"select COUNT(req.RequestId) Total, fwdsts.Name,fwdsts.FwdStatusId,mrec.RecordOfficeId, 3 GroupId from MTrnFwdStatus fwdsts
                            inner join TrnFwds fwd on fwdsts.FwdStatusId=fwd.FwdStatusId and fwd.StepId in (3,8)
                            inner join TrnICardRequest req on fwd.RequestId=req.RequestId and req.StatusId=@RunningStatusId 
                            inner join OROMapping mrec on req.RecordOfficeId=mrec.RecordOfficeId 
                            inner join  BasicDetails basi on req.BasicDetailId=basi.BasicDetailId and basi.ApplyForId=1
                            left join MapUnit unit on basi.UnitId=unit.UnitMapId 
                            where 
                            (
                                (@UnitType = 1 AND
                                    unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                    AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                    AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                    AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                )
                                OR
                                (@UnitType = 2 AND
                                    unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                    AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                    AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                    AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                    AND unit.FmnBranchID = ISNULL(@FmnBranchID, unit.FmnBranchID)
                                )
                                OR
                                (@UnitType = 3 AND
                                    unit.PsoId = ISNULL(@PsoId, unit.PsoId)
                                    AND unit.SubDteId = ISNULL(@SubDteId, unit.SubDteId)
                                )
                            )
                            AND unit.UnitMapId = ISNULL(@UnitMapId, unit.UnitMapId)
                            AND unit.UnitType =@UnitType
                            group by fwdsts.Name,fwdsts.FwdStatusId,mrec.RecordOfficeId";
            try 
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@UnitType", Data.UnitType, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@UnitMapId", Data.UnitMapId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@ComdId", Data.ComdId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@CorpsId", Data.CorpsId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@DivId", Data.DivId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@BdeId", Data.BdeId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@FmnBranchID", Data.FmnBranchID, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@PsoId", Data.PsoId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@SubDteId", Data.SubDteId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@RunningStatusId", (byte)RequestStatusEnum.Running);
                    var ret = await connection.QueryAsync<DTOReportReturnCount>(query, parameters);
                    return ret.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "ReportReturnDB->GetRecordJco");
                return new List<DTOReportReturnCount>();
            }
        }

        
        /// <summary>
        /// Retrieves a list of Record Offices excluding those with a specific ArmedId.
        /// </summary>
        /// <param name="ArmedIdForORO">The ArmedId used to exclude certain record offices.</param>
        /// <returns>Returns a list of DTOReportReturnCount representing excluded record offices.</returns>
        public async Task<List<DTOReportReturnCount>> GetRecordJco(short ArmedIdForORO)
        {
            string query = "select RecordOfficeId ,Name from MRecordOffice where ArmedId!=@ArmedIdForORO";
            try 
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTOReportReturnCount>(query,new { ArmedIdForORO });
                    return ret.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "ReportReturnDB->GetRecordJco");
                return new List<DTOReportReturnCount>();
            }
        }


        /// <summary>
        /// Retrieves the count of record offices based on request data, completion status, and ArmedId exclusion.
        /// </summary>
        /// <param name="Data">The input data for hierarchical request.</param>
        /// <param name="IsComplete">Flag indicating whether the process is complete.</param>
        /// <param name="ArmedIdForORO">The ArmedId used to filter record offices.</param>
        /// <returns>Returns a list of DTOReportReturnCount representing the filtered count of record offices.</returns>
        public async Task<List<DTOReportReturnCount>> GetRecordJcoCount(DTOMHierarchyRequest Data, int IsComplete, short ArmedIdForORO)
        {
            string query = @"select count(req.RequestId) Total ,recf.RecordOfficeId,recf.Name,step.StepId from MRecordOffice recf
                            inner join TrnICardRequest req on recf.RecordOfficeId=req.RecordOfficeId and req.StatusId=@RunningStatusId  
                            inner join TrnFwds fwd on req.RequestId=fwd.RequestId and fwd.IsComplete=@IsComplete and fwd.StepId=3
                            inner join TrnStepCounter step on req.RequestId=step.RequestId 
                            inner join BasicDetails basi on req.BasicDetailId=basi.BasicDetailId and basi.ApplyForId=2
                            left join MapUnit unit on basi.UnitId=unit.UnitMapId
                            where 
                            (
                                (@UnitType = 1 AND
                                    unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                    AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                    AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                    AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                )
                                OR
                                (@UnitType = 2 AND
                                    unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                    AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                    AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                    AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                    AND unit.FmnBranchID = ISNULL(@FmnBranchID, unit.FmnBranchID)
                                )
                                OR
                                (@UnitType = 3 AND
                                    unit.PsoId = ISNULL(@PsoId, unit.PsoId)
                                    AND unit.SubDteId = ISNULL(@SubDteId, unit.SubDteId)
                                )
                            )
                            AND unit.UnitMapId = ISNULL(@UnitMapId, unit.UnitMapId)
                            AND unit.UnitType =@UnitType
                            AND recf.ArmedId!=@ArmedIdForORO group by recf.RecordOfficeId,recf.Name,step.StepId";
            try 
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@IsComplete", IsComplete, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@ArmedIdForORO", ArmedIdForORO, DbType.Int16, ParameterDirection.Input);
                    parameters.Add("@UnitType", Data.UnitType, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@UnitMapId", Data.UnitMapId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@ComdId", Data.ComdId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@CorpsId", Data.CorpsId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@DivId", Data.DivId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@BdeId", Data.BdeId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@FmnBranchID", Data.FmnBranchID, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@PsoId", Data.PsoId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@SubDteId", Data.SubDteId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@RunningStatusId", (byte)RequestStatusEnum.Running);

                    var ret = await connection.QueryAsync<DTOReportReturnCount>(query, parameters);
                    return ret.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "ReportReturnDB->GetReportForm11");
                return new List<DTOReportReturnCount>();
            }
        }


        /// <summary>
        /// Retrieves a paginated history of records based on the request parameters and sorting options.
        /// </summary>
        /// <param name="dTORecord">The input data for record history and pagination.</param>
        /// <returns>Returns a paginated list of DTOReportReturnListResponse with sorted and filtered results.</returns>
        public async Task<DTODataTablesResponse<DTOReportReturnListResponse>> GetRecordHistory(DTORecordHistory dTORecord)
        {
            // Map allowed sort columns to DB fields
            var allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["ServiceNo"] = "ServiceNo",
                ["UpdatedOn"] = "fwd.UpdatedOn",
                ["StatusName"] = "fwdsts.Name",
            };

            var sortColumn = allowedSortColumns.ContainsKey(dTORecord.sortColumn ?? "")
                ? allowedSortColumns[dTORecord.sortColumn!]
                : "ServiceNo";

            var sortOrder = dTORecord.sortDirection;

            string query = "";
            string wherequery = "";
            if (dTORecord.StepId != 99)
            {
                if(dTORecord.StepId ==100)
                {
                    query = @"req.RequestId,Mstep.StepId,basi.FName,basi.LName,ServiceNo,DOB,ranks.RankAbbreviation RankName, 
                            aspusersto.DomainId DomainIdTo,userto.ArmyNo ArmyNoTo ,userto.Name NameTo,ranksto.RankAbbreviation RankTo, 
                            aspusersfrom.DomainId DomainIdFrom,userfrom.ArmyNo ArmyNoFrom ,userfrom.Name NameFrom,ranksfrom.RankAbbreviation RankFrom,fwdsts.Name Status
                            ,fwd.UpdatedOn,fwdsts.Name StatusName from MStepCounterStep Mstep   
                            inner join TrnStepCounter step on Mstep.StepId=step.StepId  
                            inner join TrnICardRequest req on step.RequestId=req.RequestId and req.StatusId=1  
                            inner join  BasicDetails basi on req.BasicDetailId=basi.BasicDetailId  
                            inner join TrnFwds fwd on req.RequestId=fwd.RequestId 
                            inner join MRecordOffice mrec on req.RecordOfficeId=mrec.RecordOfficeId
                            left join UserProfile userto on fwd.ToUserId=userto.UserId  
                            LEFT join TrnDomainMapping mapto on userto.UserId=mapto.UserId  
                            LEFT join AspNetUsers aspusersto on mapto.AspNetUsersId=aspusersto.Id 
                            left join MRank ranksto on ranksto.RankId=userto.RankId 
                            left join MTrnFwdStatus fwdsts on fwd.FwdStatusId=fwdsts.FwdStatusId 
                            left join UserProfile userfrom on fwd.FromUserId=userfrom.UserId  
                            LEFT join TrnDomainMapping mapfrom on userfrom.UserId=mapfrom.UserId  
                            LEFT join AspNetUsers aspusersfrom on mapfrom.AspNetUsersId=aspusersfrom.Id 
                            left join MRank ranksfrom on ranksfrom.RankId=userfrom.RankId 
                            left join MRank ranks on ranks.RankId=basi.RankId 
                            left join MapUnit unit on basi.UnitId=unit.UnitMapId";
                    
                    wherequery = @"WHERE 
                                (
                                    (@UnitType = 1 AND
                                        unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                        AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                        AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                        AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                    )
                                    OR
                                    (@UnitType = 2 AND
                                        unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                        AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                        AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                        AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                        AND unit.FmnBranchID = ISNULL(@FmnBranchID, unit.FmnBranchID)
                                    )
                                    OR
                                    (@UnitType = 3 AND
                                        unit.PsoId = ISNULL(@PsoId, unit.PsoId)
                                        AND unit.SubDteId = ISNULL(@SubDteId, unit.SubDteId)
                                    )
                                )
                                AND unit.UnitMapId = ISNULL(@UnitMapId, unit.UnitMapId)
                                AND unit.UnitType =@UnitType
                                AND step.ApplyForId=2 AND fwd.IsComplete=0 AND fwd.StepId=3 AND mrec.RecordOfficeId=@ApplyForId AND ServiceNo like '%' + @SearchTerm + '%' ";
                }
                else if (dTORecord.IsApproveId == 1)
                {
                    query = @"req.RequestId,Mstep.StepId,basi.FName,basi.LName,ServiceNo,DOB,ranks.RankAbbreviation RankName,
                            aspusersto.DomainId DomainIdTo,userto.ArmyNo ArmyNoTo ,userto.Name NameTo,ranksto.RankAbbreviation RankTo,
                            aspusersfrom.DomainId DomainIdFrom,userfrom.ArmyNo ArmyNoFrom ,userfrom.Name NameFrom,ranksfrom.RankAbbreviation RankFrom
                            ,fwd.UpdatedOn,fwdsts.Name StatusName from MStepCounterStep Mstep  
                            left join TrnStepCounter step on Mstep.StepId=step.StepId 
                            left join TrnICardRequest req on step.RequestId=req.RequestId and req.StatusId=1 
                            left join TrnFwds fwd on req.RequestId=fwd.RequestId 
                            left join UserProfile userto on fwd.ToUserId=userto.UserId
                            LEFT join TrnDomainMapping mapto on userto.UserId=mapto.UserId
                            LEFT join AspNetUsers aspusersto on mapto.AspNetUsersId=aspusersto.Id
                            left join MRank ranksto on ranksto.RankId=userto.RankId
                            left join MTrnFwdStatus fwdsts on fwd.FwdStatusId=fwdsts.FwdStatusId
                            left join UserProfile userfrom on fwd.FromUserId=userfrom.UserId
                            LEFT join TrnDomainMapping mapfrom on userfrom.UserId=mapfrom.UserId
                            LEFT join AspNetUsers aspusersfrom on mapfrom.AspNetUsersId=aspusersfrom.Id
                            left join MRank ranksfrom on ranksfrom.RankId=userfrom.RankId
                            left join BasicDetails basi on req.BasicDetailId=basi.BasicDetailId 
                            left join MRank ranks on ranks.RankId=basi.RankId
                            left join MapUnit unit on basi.UnitId=unit.UnitMapId";
                    
                    wherequery = @"WHERE 
                                (
                                    (@UnitType = 1 AND
                                        unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                        AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                        AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                        AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                    )
                                    OR
                                    (@UnitType = 2 AND
                                        unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                        AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                        AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                        AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                        AND unit.FmnBranchID = ISNULL(@FmnBranchID, unit.FmnBranchID)
                                    )
                                    OR
                                    (@UnitType = 3 AND
                                        unit.PsoId = ISNULL(@PsoId, unit.PsoId)
                                        AND unit.SubDteId = ISNULL(@SubDteId, unit.SubDteId)
                                    )
                                )
                                AND unit.UnitMapId = ISNULL(@UnitMapId, unit.UnitMapId)
                                AND unit.UnitType =@UnitType
                                AND step.ApplyForId=@ApplyForId AND step.StepId=@StepId AND fwd.StepId=@StepId AND ServiceNo like '%' + @SearchTerm + '%'";

                }
                else
                {
                    if(dTORecord.StepId ==1)
                    {
                        query = @"req.RequestId,Mstep.StepId,basi.FName,basi.LName,ServiceNo,DOB,ranks.RankAbbreviation RankName,
                                aspusersto.DomainId DomainIdTo,userto.ArmyNo ArmyNoTo ,userto.Name NameTo,ranksto.RankAbbreviation RankTo,
                                aspusersfrom.DomainId DomainIdFrom,userfrom.ArmyNo ArmyNoFrom ,userfrom.Name NameFrom,ranksfrom.RankAbbreviation RankFrom
                                ,fwd.UpdatedOn,fwdsts.Name StatusName from MStepCounterStep Mstep  
                                LEFT join TrnStepCounter step on Mstep.StepId=step.StepId and Mstep.StepId=@StepId
                                LEFT join TrnICardRequest req on step.RequestId=req.RequestId and req.StatusId=1 
                                LEFT join TrnFwds fwd on req.RequestId=fwd.RequestId and fwd.IsComplete=0 
                                LEFT join UserProfile userto on fwd.ToUserId=userto.UserId
                                LEFT join TrnDomainMapping mapto on userto.UserId=mapto.UserId
                                LEFT join AspNetUsers aspusersto on mapto.AspNetUsersId=aspusersto.Id
                                LEFT join MRank ranksto on ranksto.RankId=userto.RankId
                                LEFT join MTrnFwdStatus fwdsts on fwd.FwdStatusId=fwdsts.FwdStatusId
                                LEFT join UserProfile userfrom on fwd.FromUserId=userfrom.UserId
                                LEFT join TrnDomainMapping mapfrom on userfrom.UserId=mapfrom.UserId
                                LEFT join AspNetUsers aspusersfrom on mapfrom.AspNetUsersId=aspusersfrom.Id
                                LEFT join MRank ranksfrom on ranksfrom.RankId=userfrom.RankId
                                LEFT join BasicDetails basi on req.BasicDetailId=basi.BasicDetailId 
                                LEFT join MRank ranks on ranks.RankId=basi.RankId
                                LEFT join MapUnit unit on basi.UnitId=unit.UnitMapId";
                        
                        wherequery = @"where 
                                    (
                                        (@UnitType = 1 AND
                                            unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                            AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                            AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                            AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                        )
                                        OR
                                        (@UnitType = 2 AND
                                            unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                            AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                            AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                            AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                            AND unit.FmnBranchID = ISNULL(@FmnBranchID, unit.FmnBranchID)
                                        )
                                        OR
                                        (@UnitType = 3 AND
                                            unit.PsoId = ISNULL(@PsoId, unit.PsoId)
                                            AND unit.SubDteId = ISNULL(@SubDteId, unit.SubDteId)
                                        )
                                    )
                                    AND unit.UnitMapId = ISNULL(@UnitMapId, unit.UnitMapId)
                                    AND unit.UnitType =@UnitType
                                    AND step.ApplyForId=@ApplyForId AND ServiceNo like '%' + @SearchTerm + '%'";
                    }
                    else
                    {
                        //Appl Status at ADC
                        query = @"req.RequestId,Mstep.StepId,basi.FName,basi.LName,ServiceNo,DOB,ranks.RankAbbreviation RankName,
                                aspusersto.DomainId DomainIdTo,userto.ArmyNo ArmyNoTo ,userto.Name NameTo,ranksto.RankAbbreviation RankTo,
                                aspusersfrom.DomainId DomainIdFrom,userfrom.ArmyNo ArmyNoFrom ,userfrom.Name NameFrom,ranksfrom.RankAbbreviation RankFrom
                                ,fwd.UpdatedOn,fwdsts.Name StatusName from MStepCounterStep Mstep  
                                LEFT JOIN TrnStepCounter step on Mstep.StepId=step.StepId 
                                LEFT JOIN TrnICardRequest req on step.RequestId=req.RequestId and req.StatusId=1 
                                LEFT JOIN TrnFwds fwd on req.RequestId=fwd.RequestId and fwd.IsComplete=0 
                                LEFT JOIN UserProfile userto on fwd.ToUserId=userto.UserId
                                LEFT JOIN TrnDomainMapping mapto on userto.UserId=mapto.UserId
                                LEFT JOIN AspNetUsers aspusersto on mapto.AspNetUsersId=aspusersto.Id
                                LEFT JOIN MRank ranksto on ranksto.RankId=userto.RankId
                                LEFT JOIN MTrnFwdStatus fwdsts on fwd.FwdStatusId=fwdsts.FwdStatusId
                                LEFT JOIN UserProfile userfrom on fwd.FromUserId=userfrom.UserId
                                LEFT JOIN TrnDomainMapping mapfrom on userfrom.UserId=mapfrom.UserId
                                LEFT JOIN AspNetUsers aspusersfrom on mapfrom.AspNetUsersId=aspusersfrom.Id
                                LEFT JOIN MRank ranksfrom on ranksfrom.RankId=userfrom.RankId
                                LEFT JOIN BasicDetails basi on req.BasicDetailId=basi.BasicDetailId 
                                LEFT JOIN MRank ranks on ranks.RankId=basi.RankId
                                LEFT JOIN MapUnit unit on basi.UnitId=unit.UnitMapId";
                        
                        wherequery = @"where 
                                    (
                                        (@UnitType = 1 AND
                                            unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                            AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                            AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                            AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                        )
                                        OR
                                        (@UnitType = 2 AND
                                            unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                            AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                            AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                            AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                            AND unit.FmnBranchID = ISNULL(@FmnBranchID, unit.FmnBranchID)
                                        )
                                        OR
                                        (@UnitType = 3 AND
                                            unit.PsoId = ISNULL(@PsoId, unit.PsoId)
                                            AND unit.SubDteId = ISNULL(@SubDteId, unit.SubDteId)
                                        )
                                    )
                                    AND unit.UnitMapId = ISNULL(@UnitMapId, unit.UnitMapId)
                                    AND unit.UnitType =@UnitType
                                    AND step.ApplyForId=@ApplyForId AND fwd.StepId=@StepId AND ServiceNo like '%' + @SearchTerm + '%'";
                    }
                      
                }
            }
            else
            {
                    query = @"req.RequestId,Mstep.StepId,basi.FName,basi.LName,ServiceNo,DOB,ranks.RankAbbreviation RankName, 
                            aspusersto.DomainId DomainIdTo,userto.ArmyNo ArmyNoTo ,userto.Name NameTo,ranksto.RankAbbreviation RankTo, 
                            aspusersfrom.DomainId DomainIdFrom,userfrom.ArmyNo ArmyNoFrom ,userfrom.Name NameFrom,ranksfrom.RankAbbreviation RankFrom,fwdsts.Name Status
                            ,fwd.UpdatedOn,fwdsts.Name StatusName from MStepCounterStep Mstep   
                            inner join TrnStepCounter step on Mstep.StepId=step.StepId  and step.ApplyForId=1
                            inner join TrnICardRequest req on step.RequestId=req.RequestId and req.StatusId=1  
                            inner join TrnFwds fwd on req.RequestId=fwd.RequestId and fwd.StepId in (3,8)
                            inner join TrnDomainMapping map on fwd.ToAspNetUsersId=map.AspNetUsersId  
                            inner join OROMapping mrec on req.RecordOfficeId=mrec.RecordOfficeId
                            inner join BasicDetails basi on req.BasicDetailId=basi.BasicDetailId
                            left join UserProfile userto on fwd.ToUserId=userto.UserId  
                            LEFT join TrnDomainMapping mapto on userto.UserId=mapto.UserId  
                            LEFT join AspNetUsers aspusersto on mapto.AspNetUsersId=aspusersto.Id 
                            left join MRank ranksto on ranksto.RankId=userto.RankId 
                            left join MTrnFwdStatus fwdsts on fwd.FwdStatusId=fwdsts.FwdStatusId 
                            left join UserProfile userfrom on fwd.FromUserId=userfrom.UserId  
                            LEFT join TrnDomainMapping mapfrom on userfrom.UserId=mapfrom.UserId  
                            LEFT join AspNetUsers aspusersfrom on mapfrom.AspNetUsersId=aspusersfrom.Id 
                            left join MRank ranksfrom on ranksfrom.RankId=userfrom.RankId 
                            left join MRank ranks on ranks.RankId=basi.RankId 
                            left join MapUnit unit on basi.UnitId=unit.UnitMapId";

                    wherequery = @"where 
                                    (
                                        (@UnitType = 1 AND
                                            unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                            AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                            AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                            AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                        )
                                        OR
                                        (@UnitType = 2 AND
                                            unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                            AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                            AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                            AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                            AND unit.FmnBranchID = ISNULL(@FmnBranchID, unit.FmnBranchID)
                                        )
                                        OR
                                        (@UnitType = 3 AND
                                            unit.PsoId = ISNULL(@PsoId, unit.PsoId)
                                            AND unit.SubDteId = ISNULL(@SubDteId, unit.SubDteId)
                                        )
                                    )
                                    AND unit.UnitMapId = ISNULL(@UnitMapId, unit.UnitMapId)
                                    AND unit.UnitType =@UnitType
                                    AND mrec.RecordOfficeId=@ApplyForId AND ServiceNo like '%' + @SearchTerm + '%'";

            }
            try
            {
                var multiQuery = query = $@"
                            WITH RecordCTE AS (
                                select  Count(*) OVER () as TotalFilteredRecords,ROW_NUMBER() OVER (ORDER BY {sortColumn} {sortOrder}) AS RowNum, {query} {wherequery}
                            )
                            SELECT * FROM RecordCTE WHERE RowNum BETWEEN @Offset AND @Limit;
                        ";

                using (var connection = _contextDP.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@ApplyForId", dTORecord.ApplyForId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@StepId", dTORecord.StepId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@UnitMapId", dTORecord.Data.UnitMapId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@UnitType", dTORecord.Data.UnitType, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@ComdId", dTORecord.Data.ComdId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@CorpsId", dTORecord.Data.CorpsId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@DivId", dTORecord.Data.DivId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@BdeId", dTORecord.Data.BdeId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@FmnBranchID", dTORecord.Data.FmnBranchID, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@PsoId", dTORecord.Data.PsoId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@SubDteId", dTORecord.Data.SubDteId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@Offset", dTORecord.Start + 1, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Limit", (dTORecord.Start + dTORecord.Length), DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@SearchTerm", dTORecord.searchValue, DbType.String, ParameterDirection.Input);

                    var ret = await connection.QueryMultipleAsync(query, parameters);
                    var records = (await ret.ReadAsync<DTOReportReturnListResponse>()).ToList();
                    var totalFilteredRecords = records?.FirstOrDefault()?.TotalFilteredRecords;

                    var responseData = new DTODataTablesResponse<DTOReportReturnListResponse>
                    {
                        draw = dTORecord.Draw,
                        recordsTotal = totalFilteredRecords.GetValueOrDefault(),
                        recordsFiltered = totalFilteredRecords.GetValueOrDefault(),
                        data = records
                    };
                    return responseData;
                }
            }
            catch (Exception ex) 
            {
                _logger.LogError(1001, ex, "ReportReturnDB->GetRecordHistory");
                List<DTOReportReturnListResponse> dTOUserRegnResponses = new List<DTOReportReturnListResponse>();
                var responseData = new DTODataTablesResponse<DTOReportReturnListResponse>
                {
                    draw = dTORecord.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTOUserRegnResponses
                };
                return responseData;
            }
        }

        
        /// <summary>
        /// Retrieves report data based on the provided filter and sorting options.
        /// This method fetches filtered and sorted data from multiple tables like `TrnStepCounter`, `MApplyFor`, `MStepCounterStep`, `TrnICardRequest`, and more.
        /// </summary>
        /// <param name="dTO">The request object containing filter, sort, and pagination details.</param>
        /// <returns>A response object containing the filtered and paginated report data.</returns>
        /// <exception cref="Exception">Thrown when an error occurs during data retrieval.</exception>
        public async Task<DTODataTablesResponse<DTOReportResponse>> GetReportData(DTODataTablesRequestForReport dTO)
        {
            string query = "";
            string wherequery = "";
            // Map allowed sort columns to DB fields
            Dictionary<string, string> allowedSortColumns = new Dictionary<string, string>();

            var sortOrder = dTO.sortDirection == "desc" ? "DESC" : "ASC";
            if (dTO.Choice == "Requisition")
            {
                allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["ServiceNo"] = "ServiceNo",
                    ["RequestId"] = "req.RequestId",
                    ["StepId"] = "Mstep.StepId",
                    ["ArmedAbbreviation"] = "marmed.Abbreviation",
                    ["ApplyFor"] = "mappl.Name"
                };
                query = @"req.RequestId,Mstep.StepId,basi.FName,basi.LName,basi.NameAsPerRecord,ServiceNo,ranks.RankAbbreviation RankName,marmed.Abbreviation as ArmedAbbreviation,regi.Abbreviation RegimentalName,mappl.Name as ApplyFor,
                            REPLACE(Mstep.Name, '</br>', '') as Status
                            from TrnStepCounter step
                            INNER JOIN MApplyFor mappl on mappl.ApplyForId=step.ApplyForId
                            INNER JOIN MStepCounterStep Mstep on Mstep.StepId=step.StepId
                            INNER JOIN TrnICardRequest req on step.RequestId=req.RequestId and req.StatusId=@RunningStatusId
                            INNER JOIN BasicDetails basi on req.BasicDetailId=basi.BasicDetailId
                            INNER JOIN MArmedType marmed on basi.ArmedId=marmed.ArmedId
                            INNER JOIN MRank ranks on ranks.RankId=basi.RankId
                            INNER JOIN MapUnit unit on basi.UnitId=unit.UnitMapId
                            left join MRegimental regi on regi.RegId=basi.RegimentalId ";
                wherequery = @"WHERE
                            (
                                (@UnitType = 1 AND
                                    unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                    AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                    AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                    AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                )
                                OR
                                (@UnitType = 2 AND
                                    unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                    AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                    AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                    AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                    AND unit.FmnBranchID = ISNULL(@FmnBranchID, unit.FmnBranchID)
                                )
                                OR
                                (@UnitType = 3 AND
                                    unit.PsoId = ISNULL(@PsoId, unit.PsoId)
                                    AND unit.SubDteId = ISNULL(@SubDteId, unit.SubDteId)
                                )
                            )
							AND unit.UnitType =@UnitType
                            AND unit.UnitMapId = ISNULL(@UnitMapId, unit.UnitMapId)
                            AND 
                            (
                                @SearchTerm IS NULL OR 
                                ServiceNo LIKE @SearchTerm
                            )";
            }
            else if (dTO.Choice == "NonFunctional")
            {
                allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["ServiceNo"] = "ServiceNo",
                    ["RequestId"] = "req.RequestId",
                    ["ArmedAbbreviation"] = "marmed.Abbreviation",
                    ["ApplyFor"] = "appl.Name"
                };
                query = @"appl.Name ApplyFor,marmed.Abbreviation as ArmedAbbreviation,regi.Abbreviation RegimentalName,mcat.Name FaultyStage,req.RequestId,
                            ISNULL(bd.ServiceNo, basic_2.ServiceNo) AS ServiceNo,ranks.RankAbbreviation RankName,bd.FName AS FName_1,bd.LName AS LName_1,basic_2.FName AS FName_2,basic_2.LName AS LName_2,
                            Muni.Abbreviation UnitAbbreviation,faulty.UpdatedOn,faulty.FromRemark,faulty.ToRemark,
                            (select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(faulty.RemarksIds,','))) RemarksNameList
                            from TrnFaultyCard faulty
                            inner join MCategory mcat on mcat.CategoryId = faulty.CategoryId
                            inner join TrnICardRequest req on req.RequestId = faulty.RequestId
                            inner join TrnDomainMapping tdm on tdm.Id=req.TrnDomainMappingId
                            LEFT JOIN AFSAC2.dbo.BasicDetails basic_2 on basic_2.BasicDetailId=req.BasicDetailId
                            LEFT JOIN BasicDetails bd on bd.BasicDetailId=req.BasicDetailId
							INNER JOIN MArmedType marmed on marmed.ArmedId = ISNULL(basic_2.ArmedId,bd.ArmedId)
                            inner join MRank ranks on ranks.RankId = ISNULL(basic_2.RankId,bd.RankId)
                            inner join MapUnit unit on unit.UnitMapId = ISNULL(basic_2.UnitId,bd.UnitId)
                            inner join MUnit Muni on Muni.UnitId = unit.UnitId
                            inner join MApplyFor appl on appl.ApplyForId = ISNULL(basic_2.ApplyForId,bd.ApplyForId)
                            left join MRegimental regi on regi.RegId = ISNULL(basic_2.RegimentalId,bd.RegimentalId)";
                wherequery = @"WHERE
                            (
                                (@UnitType = 1 AND
                                    unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                    AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                    AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                    AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                )
                                OR
                                (@UnitType = 2 AND
                                    unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                    AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                    AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                    AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                    AND unit.FmnBranchID = ISNULL(@FmnBranchID, unit.FmnBranchID)
                                )
                                OR
                                (@UnitType = 3 AND
                                    unit.PsoId = ISNULL(@PsoId, unit.PsoId)
                                    AND unit.SubDteId = ISNULL(@SubDteId, unit.SubDteId)
                                )
                            )
							AND unit.UnitType =@UnitType
                            AND unit.UnitMapId = ISNULL(@UnitMapId, unit.UnitMapId)
                            AND 
                            (
                                @SearchTerm IS NULL OR 
                                bd.ServiceNo LIKE @SearchTerm OR
                                basic_2.ServiceNo LIKE @SearchTerm
                            )";
            }
            else if (dTO.Choice == "LostCase")
            {
                allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["ServiceNo"] = "ServiceNo",
                    ["RequestId"] = "req.RequestId",
                    ["ArmedAbbreviation"] = "marmed.Abbreviation",
                    ["ApplyFor"] = "appl.Name",
                    ["IsFIRLogged"] = "lost.IsFIRLogged",
                    ["LostOn"] = "lost.LostOn",
                    ["UpdatedOn"] = "lost.UpdatedOn"
                };
                query = @"appl.Name ApplyFor,marmed.Abbreviation as ArmedAbbreviation,req.RequestId,basic_2.ServiceNo,ranks.RankAbbreviation RankName,basic_2.FName,basic_2.LName
                            Muni.Abbreviation UnitAbbreviation,lost.UpdatedOn,lost.Remark as FromRemark,lost.LostOn,regi.Abbreviation RegimentalName,lost.IsFIRLogged,lost.SupportDocName
                            from TrnLostCards lost
                            inner join TrnICardRequest req on req.RequestId = lost.RequestId
                            inner join TrnDomainMapping tdm on tdm.Id=req.TrnDomainMappingId
                            inner join AFSAC2.dbo.BasicDetails basic_2 on basic_2.BasicDetailId=req.BasicDetailId
		                    INNER JOIN MArmedType marmed on marmed.ArmedId = basic_2.ArmedId 
                            inner join MRank ranks on ranks.RankId = basic_2.RankId
                            inner join MapUnit unit on unit.UnitMapId = basic_2.UnitId 
                            inner join MUnit Muni on Muni.UnitId=unit.UnitId
                            inner join MApplyFor appl on appl.ApplyForId = basic_2.ApplyForId
                            left join MRegimental regi on regi.RegId = basic_2.RegimentalId ";
                wherequery = @"WHERE
                            (
                                (@UnitType = 1 AND
                                    unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                    AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                    AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                    AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                )
                                OR
                                (@UnitType = 2 AND
                                    unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                    AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                    AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                    AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                    AND unit.FmnBranchID = ISNULL(@FmnBranchID, unit.FmnBranchID)
                                )
                                OR
                                (@UnitType = 3 AND
                                    unit.PsoId = ISNULL(@PsoId, unit.PsoId)
                                    AND unit.SubDteId = ISNULL(@SubDteId, unit.SubDteId)
                                )
                            )
							AND unit.UnitType =@UnitType
                            AND unit.UnitMapId = ISNULL(@UnitMapId, unit.UnitMapId)
                            AND (
                                @SearchTerm IS NULL OR 
                                basic_2.ServiceNo LIKE @SearchTerm
                            )";
            }
            else if (dTO.Choice == "MonthlyProcessed")
            {
                allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["ServiceNo"] = "ServiceNo",
                    ["RequestId"] = "req.RequestId",
                    ["StepId"] = "Mstep.StepId",
                    ["ArmedAbbreviation"] = "marmed.Abbreviation",
                    ["ApplyFor"] = "mappl.Name"
                };
                query = @"req.RequestId,Mstep.StepId,basi.FName,basi.LName,basi.NameAsPerRecord,ServiceNo,ranks.RankAbbreviation RankName,marmed.Abbreviation as ArmedAbbreviation,regi.Abbreviation RegimentalName,mappl.Name as ApplyFor,basi.UpdatedOn,
                            REPLACE(Mstep.Name, '</br>', '') as Status
                            from TrnStepCounter step
                            INNER JOIN MApplyFor mappl on mappl.ApplyForId=step.ApplyForId
                            INNER JOIN MStepCounterStep Mstep on Mstep.StepId=step.StepId
                            INNER JOIN TrnICardRequest req on step.RequestId=req.RequestId and req.StatusId=@RunningStatusId
                            INNER JOIN  BasicDetails basi on req.BasicDetailId=basi.BasicDetailId
                            INNER JOIN MArmedType marmed on basi.ArmedId=marmed.ArmedId
                            INNER JOIN MRank ranks on ranks.RankId=basi.RankId
                            INNER JOIN MapUnit unit on basi.UnitId=unit.UnitMapId
                            left join MRegimental regi on regi.RegId=basi.RegimentalId ";
                wherequery = @"WHERE
                            (
                                (@UnitType = 1 AND
                                    unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                    AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                    AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                    AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                )
                                OR
                                (@UnitType = 2 AND
                                    unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                    AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                    AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                    AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                    AND unit.FmnBranchID = ISNULL(@FmnBranchID, unit.FmnBranchID)
                                )
                                OR
                                (@UnitType = 3 AND
                                    unit.PsoId = ISNULL(@PsoId, unit.PsoId)
                                    AND unit.SubDteId = ISNULL(@SubDteId, unit.SubDteId)
                                )
                            )
							AND unit.UnitType =@UnitType
                            AND unit.UnitMapId = ISNULL(@UnitMapId, unit.UnitMapId)
                            AND YEAR(basi.UpdatedOn) = RIGHT(@MonthYear, 4)
							AND MONTH(basi.UpdatedOn) = LEFT(@MonthYear, 2)
                            AND 
                            (
                                @SearchTerm IS NULL OR 
                                ServiceNo LIKE @SearchTerm
                            )";
            }
            try
            {
                var sortColumn = allowedSortColumns.ContainsKey(dTO.sortColumn ?? "") ? allowedSortColumns[dTO.sortColumn!] : "req.RequestId";
                if (dTO.Choice == "NonFunctional")
                {
                    if (string.Equals(dTO.sortColumn, "ServiceNo", StringComparison.OrdinalIgnoreCase))
                    {
                        sortColumn = "ISNULL(basic_2.ServiceNo , bd.ServiceNo )";
                    }
                }

                var multiQuery = query = $@"
                        WITH RecordCTE AS (
                            select  Count(*) OVER () as TotalFilteredRecords,ROW_NUMBER() OVER (ORDER BY {sortColumn} {sortOrder}) AS RowNum, {query} {wherequery}
                        )
                        SELECT * FROM RecordCTE WHERE RowNum BETWEEN @Offset AND @Limit;";

                using (var connection = _contextDP.CreateConnection())
                {
                    var searchTerm = string.IsNullOrEmpty(dTO.searchValue) ? null : $"%{dTO.searchValue.Trim()}%";

                    var parameters = new DynamicParameters();
                    parameters.Add("@UnitMapId", dTO.UnitMapId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@UnitType", dTO.UnitType, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@ComdId", dTO.ComdId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@CorpsId", dTO.CorpsId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@DivId", dTO.DivId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@BdeId", dTO.BdeId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@FmnBranchID", dTO.FmnBranchID, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@PsoId", dTO.PsoId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@SubDteId", dTO.SubDteId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@MonthYear", dTO.MonthYear, DbType.String, ParameterDirection.Input);
                    parameters.Add("@Offset", dTO.Start + 1, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Limit", (dTO.Start + dTO.Length) , DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@SearchTerm", searchTerm, DbType.String, ParameterDirection.Input);
                    parameters.Add("@RunningStatusId", (byte)RequestStatusEnum.Running);

                    var ret = await connection.QueryMultipleAsync(query, parameters);
                    var records = (await ret.ReadAsync<DTOReportResponse>()).ToList();
                    var totalFilteredRecords = records?.FirstOrDefault()?.TotalFilteredRecords;

                    if (dTO.Choice == "NonFunctional")
                    {
                        if (records != null)
                        {
                            foreach (var item in records)
                            {
                                item.FName = item.FName_2 ?? item.FName_1 ?? string.Empty;
                                item.LName = item.LName_2 ?? item.LName_1;
                            }
                        }
                    }

                    var responseData = new DTODataTablesResponse<DTOReportResponse>
                    {
                        draw = dTO.Draw,
                        recordsTotal = totalFilteredRecords.GetValueOrDefault(), 
                        recordsFiltered = totalFilteredRecords.GetValueOrDefault(),
                        data = records,
                    };
                    return responseData;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "ReportReturnDB->GetReportData");
                List<DTOReportResponse> dTOUserRegnResponses = new List<DTOReportResponse>();
                var responseData = new DTODataTablesResponse<DTOReportResponse>
                {
                    draw = dTO.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTOUserRegnResponses
                };
                return responseData;
            }
        }

        
        /// <summary>
        /// Retrieves the dashboard count for reports based on the provided hierarchy request.
        /// </summary>
        /// <param name="dTO">The hierarchy request containing filter parameters such as UnitMapId, UnitType, ComdId, etc.</param>
        /// <returns>Returns a <see cref="DTOReportDashboardCountResponse"/> object with counts for requisitions, lost cases, monthly processed, and non-functional cards.</returns>
        public async Task<DTOReportDashboardCountResponse> GetReportDashboardCount(DTOMHierarchyRequest dTO)
        {
            string query = @"declare @TotRequisition int=0
                            declare @TotLostCases int=0
                            declare @TotMonthlyProcessed int=0
                            declare @TotNonFunctionalCard int=0
                            DECLARE @MonthStart date = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);
                            DECLARE @NextMonthStart date = DATEADD(MONTH, 1, @MonthStart);

                            ;WITH FilteredUnits AS
                            (
                                SELECT UnitMapId
                                FROM MapUnit unit
                                WHERE unit.UnitType = @UnitType
                                  AND (@UnitMapId IS NULL OR unit.UnitMapId = @UnitMapId)
                                  AND
                                  (
                                        (@UnitType = 1
                                            AND (@ComdId IS NULL OR unit.ComdId = @ComdId)
                                            AND (@CorpsId IS NULL OR unit.CorpsId = @CorpsId)
                                            AND (@DivId IS NULL OR unit.DivId = @DivId)
                                            AND (@BdeId IS NULL OR unit.BdeId = @BdeId)
                                        )
                                     OR (@UnitType = 2
                                            AND (@ComdId IS NULL OR unit.ComdId = @ComdId)
                                            AND (@CorpsId IS NULL OR unit.CorpsId = @CorpsId)
                                            AND (@DivId IS NULL OR unit.DivId = @DivId)
                                            AND (@BdeId IS NULL OR unit.BdeId = @BdeId)
                                            AND (@FmnBranchID IS NULL OR unit.FmnBranchID = @FmnBranchID)
                                        )
                                     OR (@UnitType = 3
                                            AND (@PsoId IS NULL OR unit.PsoId = @PsoId)
                                            AND (@SubDteId IS NULL OR unit.SubDteId = @SubDteId)
                                        )
                                  )
                            ),
                            BaseRequests AS
                            (
                                SELECT req.RequestId, basi.UpdatedOn FROM TrnICardRequest req
                                INNER JOIN BasicDetails basi ON basi.BasicDetailId = req.BasicDetailId
                                INNER JOIN FilteredUnits unit ON unit.UnitMapId = basi.UnitId
                                WHERE req.StatusId = @RunningStatusId
                            )
                            SELECT
                                TotRequisition =
                                (
                                    SELECT COUNT(DISTINCT RequestId)
                                    FROM BaseRequests
                                ),

                                TotMonthlyProcessed =
                                (
                                    SELECT COUNT(DISTINCT RequestId)
                                    FROM BaseRequests
                                    WHERE UpdatedOn >= @MonthStart
                                      AND UpdatedOn < @NextMonthStart
                                ),

                                TotLostCases =
                                (
                                    SELECT COUNT(DISTINCT req.RequestId)
                                    FROM TrnLostCards lost
                                    INNER JOIN TrnICardRequest req ON req.RequestId = lost.RequestId
                                    INNER JOIN AFSAC2.dbo.BasicDetails basic_2  ON basic_2.BasicDetailId = req.BasicDetailId
                                    INNER JOIN FilteredUnits unit ON unit.UnitMapId = basic_2.UnitId
                                ),

                                TotNonFunctionalCard =
                                (
                                    SELECT COUNT(DISTINCT faulty.TrnFaultyCardId)
                                    FROM TrnFaultyCard faulty
                                    INNER JOIN TrnICardRequest req ON req.RequestId = faulty.RequestId
                                    LEFT JOIN BasicDetails bd ON bd.BasicDetailId = req.BasicDetailId
                                    LEFT JOIN AFSAC2.dbo.BasicDetails basic_2 on basic_2.BasicDetailId = req.BasicDetailId
                                    INNER JOIN FilteredUnits unit ON unit.UnitMapId = ISNULL(basic_2.UnitId,bd.UnitId)
                                )
                            OPTION (RECOMPILE)";

            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@UnitMapId", dTO.UnitMapId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@UnitType", dTO.UnitType, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@ComdId", dTO.ComdId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@CorpsId", dTO.CorpsId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@DivId", dTO.DivId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@BdeId", dTO.BdeId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@FmnBranchID", dTO.FmnBranchID, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@PsoId", dTO.PsoId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@SubDteId", dTO.SubDteId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@RunningStatusId", (byte)RequestStatusEnum.Running);

                    var ret = await connection.QueryAsync<DTOReportDashboardCountResponse>(query, parameters);
                    return ret.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "ReportReturnDB->GetReportDashboardCount");
                return null;
            }
        }

        /// <summary>
        /// Retrieves a list of report form entries (Form 11) based on the provided hierarchy request.
        /// </summary>
        /// <param name="Data">The hierarchy request containing filter parameters such as ComdId, CorpsId, DivId, etc.</param>
        /// <returns>Returns a list of <see cref="DTOReportReturnListResponse"/> objects for the requested report form.</returns>
        public async Task<List<DTOReportReturnListResponse>> GetReportForm11(DTOMHierarchyRequest Data)
        {
            string query = @"select 
                              req.RequestId, 
                              basi.FName,basi.LName, 
                              ServiceNo, 
                              DOB, 
                              ranks.RankAbbreviation RankName 
                            from 
                              MStepCounterStep Mstep 
                              inner join TrnStepCounter step on Mstep.StepId = step.StepId 
                              inner join TrnICardRequest req on step.RequestId = req.RequestId   and req.StatusId = 1 
                              inner join BasicDetails basi on req.BasicDetailId = basi.BasicDetailId 
                              left join TrnFwds fwd on req.RequestId = fwd.RequestId 
                              left join MTrnFwdStatus fwdsts on fwd.FwdStatusId = fwdsts.FwdStatusId 
                              left join MRank ranks on ranks.RankId = basi.RankId 
                              left join MapUnit unit on basi.UnitId = unit.UnitMapId 
                            where unit.ComdId=ISNULL(@ComdId,unit.ComdId) 
                            and unit.CorpsId=ISNULL(@CorpsId,unit.CorpsId)
                            and unit.DivId=ISNULL(@DivId,unit.DivId)
                            and unit.BdeId=ISNULL(@BdeId,unit.BdeId)
                            and unit.UnitMapId=ISNULL(@UnitMapId,unit.UnitMapId)";
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTOReportReturnListResponse>(query, new { Data.ComdId, Data.CorpsId, Data.DivId, Data.BdeId, Data.FmnBranchID, Data.PsoId, Data.SubDteId, Data.UnitMapId });
                    return ret.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "ReportReturnDB->GetReportForm11");
                return new List<DTOReportReturnListResponse>();
            }
        }

        public async Task<DTOReportCardDashboardCountResponse> GetReportCardDashboardCount(DTOMHierarchyRequest dTO)
        {
            string query = @"DECLARE @TotExported_Officer int
                            DECLARE @TotPrinted_Officer int
                            DECLARE @TotDispatchToORO int
                            DECLARE @TotCardInORO int
                            DECLARE @TotDispatchToUnit_Officer int
                            DECLARE @TotCardInUnit_Officer int
                            DECLARE @TotDistributed_Officer int

                            DECLARE @TotExported_OR int
                            DECLARE @TotPrinted_OR int
                            DECLARE @TotDispatchToRegt int
                            DECLARE @TotCardInRegt int
                            DECLARE @TotDispatchToUnit_OR int
                            DECLARE @TotCardInUnit_OR int
                            DECLARE @TotDistributed_OR int
							

                            SET @TotExported_Officer=0
                            SET @TotPrinted_Officer=0
                            SET @TotDispatchToORO=0
                            SET @TotCardInORO=0
                            SET @TotDispatchToUnit_Officer=0
                            SET @TotCardInUnit_Officer=0
                            SET @TotDistributed_Officer=0

                            SET @TotExported_OR=0
                            SET @TotPrinted_OR=0
                            SET @TotDispatchToRegt=0
                            SET @TotCardInRegt=0
                            SET @TotDispatchToUnit_OR=0
                            SET @TotCardInUnit_OR=0
                            SET @TotDistributed_OR=0

                            -- Calculation for TotExported_Officer, TotExported_OR, TotPrinted_Officer, TotPrinted_OR
                            -- Ensure there is a semicolon before the WITH clause
                            ;WITH CTE AS
                            (
                                -- Current DB data: StatusId = 1 or 2
                                SELECT 
                                    stcount.RequestId,
                                    basi.ApplyForId,
                                    stcount.StepId,
                                    req.StatusId,
                                    unit.UnitMapId
                                FROM TrnStepCounter stcount
                                INNER JOIN TrnICardRequest req ON stcount.RequestId = req.RequestId AND req.StatusId = 1  AND stcount.StepId IN (5,6,11,12,13,14)
                                INNER JOIN BasicDetails basi ON req.BasicDetailId = basi.BasicDetailId
                                INNER JOIN MapUnit unit ON basi.UnitId = unit.UnitMapId
                                WHERE unit.UnitMapId = ISNULL(@UnitMapId, unit.UnitMapId)
                                  AND unit.UnitType = @UnitType
                                  AND
                                  (
                                        (@UnitType = 1
                                            AND unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                            AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                            AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                            AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                        )
                                     OR (@UnitType = 2
                                            AND unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                            AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                            AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                            AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                            AND unit.FmnBranchID = ISNULL(@FmnBranchID, unit.FmnBranchID)
                                        )
                                     OR (@UnitType = 3
                                            AND unit.PsoId = ISNULL(@PsoId, unit.PsoId)
                                            AND unit.SubDteId = ISNULL(@SubDteId, unit.SubDteId)
                                        )
                                  )

                                UNION ALL

                                -- AFSAC2 DB data: StatusId = 2
                                SELECT 
                                    stcount.RequestId,
                                    basi2.ApplyForId,
                                    stcount.StepId,
                                    req.StatusId,
                                    unit.UnitMapId
                                FROM TrnStepCounter stcount
                                INNER JOIN TrnICardRequest req ON stcount.RequestId = req.RequestId AND req.StatusId = 2
                                INNER JOIN AFSAC2.dbo.BasicDetails basi2 ON req.BasicDetailId = basi2.BasicDetailId
                                INNER JOIN MapUnit unit ON basi2.UnitId = unit.UnitMapId
                                WHERE stcount.StepId = 15
                                  AND unit.UnitMapId = ISNULL(@UnitMapId, unit.UnitMapId)
                                  AND unit.UnitType = @UnitType
                                  AND
                                  (
                                        (@UnitType = 1
                                            AND unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                            AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                            AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                            AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                        )
                                     OR (@UnitType = 2
                                            AND unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                            AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                            AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                            AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                            AND unit.FmnBranchID = ISNULL(@FmnBranchID, unit.FmnBranchID)
                                        )
                                     OR (@UnitType = 3
                                            AND unit.PsoId = ISNULL(@PsoId, unit.PsoId)
                                            AND unit.SubDteId = ISNULL(@SubDteId, unit.SubDteId)
                                        )
                                  )
                            )
                            SELECT 
                                ISNULL(SUM(CASE WHEN ApplyForId = 1 AND StepId = 5  AND StatusId = 1 THEN 1 ELSE 0 END), 0) AS TotExported_Officer,
                                ISNULL(SUM(CASE WHEN ApplyForId = 2 AND StepId = 5  AND StatusId = 1 THEN 1 ELSE 0 END), 0) AS TotExported_OR,

                                ISNULL(SUM(CASE WHEN ApplyForId = 1 AND StepId = 6  AND StatusId = 1 THEN 1 ELSE 0 END), 0) AS TotPrinted_Officer,
                                ISNULL(SUM(CASE WHEN ApplyForId = 2 AND StepId = 6  AND StatusId = 1 THEN 1 ELSE 0 END), 0) AS TotPrinted_OR,

                                ISNULL(SUM(CASE WHEN ApplyForId = 1 AND StepId = 11 AND StatusId = 1 THEN 1 ELSE 0 END), 0) AS TotDispatchToORO,
                                ISNULL(SUM(CASE WHEN ApplyForId = 2 AND StepId = 11 AND StatusId = 1 THEN 1 ELSE 0 END), 0) AS TotDispatchToRegt,

                                ISNULL(SUM(CASE WHEN ApplyForId = 1 AND StepId = 12 AND StatusId = 1 THEN 1 ELSE 0 END), 0) AS TotCardInORO,
                                ISNULL(SUM(CASE WHEN ApplyForId = 2 AND StepId = 12 AND StatusId = 1 THEN 1 ELSE 0 END), 0) AS TotCardInRegt,

                                ISNULL(SUM(CASE WHEN ApplyForId = 1 AND StepId = 13 AND StatusId = 1 THEN 1 ELSE 0 END), 0) AS TotDispatchToUnit_Officer,
                                ISNULL(SUM(CASE WHEN ApplyForId = 2 AND StepId = 13 AND StatusId = 1 THEN 1 ELSE 0 END), 0) AS TotDispatchToUnit_OR,

                                ISNULL(SUM(CASE WHEN ApplyForId = 1 AND StepId = 14 AND StatusId = 1 THEN 1 ELSE 0 END), 0) AS TotCardInUnit_Officer,
                                ISNULL(SUM(CASE WHEN ApplyForId = 2 AND StepId = 14 AND StatusId = 1 THEN 1 ELSE 0 END), 0) AS TotCardInUnit_OR,

                                ISNULL(SUM(CASE WHEN ApplyForId = 1 AND StepId = 15 AND StatusId = 2 THEN 1 ELSE 0 END), 0) AS TotDistributed_Officer,
                                ISNULL(SUM(CASE WHEN ApplyForId = 2 AND StepId = 15 AND StatusId = 2 THEN 1 ELSE 0 END), 0) AS TotDistributed_OR
                            FROM CTE
                            OPTION (RECOMPILE);";

            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@UnitMapId", dTO.UnitMapId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@UnitType", dTO.UnitType, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@ComdId", dTO.ComdId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@CorpsId", dTO.CorpsId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@DivId", dTO.DivId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@BdeId", dTO.BdeId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@FmnBranchID", dTO.FmnBranchID, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@PsoId", dTO.PsoId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@SubDteId", dTO.SubDteId, DbType.Byte, ParameterDirection.Input);

                    var ret = await connection.QueryAsync<DTOReportCardDashboardCountResponse>(query, parameters);
                    return ret.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "ReportReturnDB->GetReportDashboardCount");
                return new DTOReportCardDashboardCountResponse();
            }
        }

        public async Task<DTODataTablesResponse<DTOReportCardResponse>> GetReportCardData(DTODataTablesRequestForReportCard dTO)
        {
            string selectFields = "";
            string fromJoinClause = "";
            string whereClause = "";
            // Map allowed sort columns to DB fields
            Dictionary<string, string> allowedSortColumns = new Dictionary<string, string>();


            // Common unit filter reused in all choices
            string unitFilter = @"
                                (
                                    (@UnitType = 1 AND
                                        unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                        AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                        AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                        AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                    )
                                    OR
                                    (@UnitType = 2 AND
                                        unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                        AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                        AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                        AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                        AND unit.FmnBranchID = ISNULL(@FmnBranchID, unit.FmnBranchID)
                                    )
                                    OR
                                    (@UnitType = 3 AND
                                        unit.PsoId = ISNULL(@PsoId, unit.PsoId)
                                        AND unit.SubDteId = ISNULL(@SubDteId, unit.SubDteId)
                                    )
                                )
                                AND unit.UnitMapId = ISNULL(@UnitMapId, unit.UnitMapId)";

            var sortOrder = dTO.sortDirection == "desc" ? "DESC" : "ASC";
            if (dTO.Choice == "Export")
            {
                allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["RequestId"] = "req.RequestId",
                    ["ArmedAbbreviation"] = "marmed.Abbreviation",
                    ["ServiceNo"] = "ServiceNo",
                    ["ActionOn"] = "req.CardExportedOn"
                };
                selectFields = $@"req.RequestId,basi.FName,basi.LName,ServiceNo,ranks.RankAbbreviation RankName,marmed.Abbreviation as ArmedAbbreviation,req.CardExportedOn as ActionOn";
                fromJoinClause = $@"from TrnStepCounter step
                                    INNER JOIN TrnICardRequest req on step.RequestId=req.RequestId AND req.StatusId=1 AND step.StepId=5 
                                    INNER JOIN BasicDetails basi on req.BasicDetailId=basi.BasicDetailId AND basi.ApplyForId=@ApplyForId
                                    INNER JOIN MArmedType marmed on basi.ArmedId=marmed.ArmedId
                                    INNER JOIN MRank ranks on ranks.RankId=basi.RankId
                                    INNER JOIN MapUnit unit on basi.UnitId=unit.UnitMapId";
                whereClause = $@"WHERE
                            {unitFilter}
                            AND (@SearchTerm IS NULL OR ServiceNo LIKE @SearchTerm)";
            }
            else if (dTO.Choice == "Printed")
            {
                allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["RequestId"] = "req.RequestId",
                    ["ArmedAbbreviation"] = "marmed.Abbreviation",
                    ["ServiceNo"] = "ServiceNo",
                    ["ActionOn"] = "req.CardPrintedOn"
                };
                selectFields = $@"req.RequestId,basi.FName,basi.LName,ServiceNo,ranks.RankAbbreviation RankName,marmed.Abbreviation as ArmedAbbreviation,req.CardPrintedOn as ActionOn";
                fromJoinClause = $@"from TrnStepCounter step
                                    INNER JOIN TrnICardRequest req on step.RequestId=req.RequestId AND req.StatusId=1 AND step.StepId=6
                                    INNER JOIN BasicDetails basi on req.BasicDetailId=basi.BasicDetailId AND basi.ApplyForId=@ApplyForId
                                    INNER JOIN MArmedType marmed on basi.ArmedId=marmed.ArmedId
                                    INNER JOIN MRank ranks on ranks.RankId=basi.RankId
                                    INNER JOIN MapUnit unit on basi.UnitId=unit.UnitMapId";
                whereClause = $@"WHERE
                            {unitFilter}
                            AND (@SearchTerm IS NULL OR ServiceNo LIKE @SearchTerm)";
            }
            else if (dTO.Choice == "DispatchToORO_Regt")
            {
                allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["RequestId"] = "req.RequestId",
                    ["ArmedAbbreviation"] = "marmed.Abbreviation",
                    ["ServiceNo"] = "ServiceNo",
                    ["ActionOn"] = "dcard.OutDate"
                };
                selectFields = $@"req.RequestId,ranks.RankAbbreviation as RankName ,basi.FName,basi.LName,basi.ServiceNo,marmed.Abbreviation as ArmedAbbreviation,dcard.OutDate as ActionOn,fromRanks.RankAbbreviation as FromRankName,fromUp.Name as FromName,toRanks.RankAbbreviation as ToRankName,toUp.Name as ToName,fromUp.ArmyNo as FromServiceNo,toUp.ArmyNo as ToServiceNo,fromAspUser.DomainId as FromDID,toAspUser.DomainId as ToDID";
                fromJoinClause = $@"from TrnDispatchCardMapping dcm --Card Dispatch to Regiment / Officer Record Office
                                    INNER JOIN TrnDispatchCard dcard on dcm.DispatchCardId =dcard.DispatchCardId AND dcard.Step=1 AND dcard.ApplyForId=@ApplyForId
                                    INNER JOIN TrnICardRequest req on dcm.RequestId=req.RequestId AND req.StatusId=1
                                    INNER JOIN TrnStepCounter step on req.RequestId=step.RequestId AND step.StepId=11
                                    INNER JOIN BasicDetails basi on req.BasicDetailId=basi.BasicDetailId
                                    INNER JOIN MArmedType marmed on basi.ArmedId=marmed.ArmedId
                                    INNER JOIN MRank ranks on ranks.RankId=basi.RankId
                                    INNER JOIN MapUnit unit on basi.UnitId=unit.UnitMapId
                                    INNER JOIN UserProfile fromUp on dcard.FromUserId=fromUp.UserId
                                    INNER JOIN MRank fromRanks on fromUp.RankId=fromRanks.RankId
                                    INNER JOIN UserProfile toUp on dcard.ToUserId=toUp.UserId
                                    INNER JOIN MRank toRanks on toUp.RankId=toRanks.RankId
                                    INNER JOIN AspNetUsers fromAspUser on dcard.FromAspNetUsersId = fromAspUser.Id
                                    INNER JOIN AspNetUsers toAspUser on dcard.ToAspNetUsersId = toAspUser.Id";
                whereClause = $@"WHERE
                            {unitFilter}
                            AND (@SearchTerm IS NULL OR basi.ServiceNo LIKE @SearchTerm)";
            }
            else if (dTO.Choice == "CardInORO_Regt")
            {
                allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["RequestId"] = "req.RequestId",
                    ["ArmedAbbreviation"] = "marmed.Abbreviation",
                    ["ServiceNo"] = "ServiceNo",
                    ["ActionOn"] = "dcard.ReceiptDate"
                };
                selectFields = $@"req.RequestId,ranks.RankAbbreviation as RankName ,basi.FName,basi.LName,basi.ServiceNo,marmed.Abbreviation as ArmedAbbreviation,dcard.ReceiptDate as ActionOn,fromRanks.RankAbbreviation as FromRankName,fromUp.Name as FromName,toRanks.RankAbbreviation as ToRankName,toUp.Name as ToName,fromUp.ArmyNo as FromServiceNo,toUp.ArmyNo as ToServiceNo,fromAspUser.DomainId as FromDID,toAspUser.DomainId as ToDID";
                fromJoinClause = $@"from TrnDispatchCardMapping dcm --Card in Regiment / Officer Record Office
                                    INNER JOIN TrnDispatchCard dcard on dcm.DispatchCardId =dcard.DispatchCardId AND dcard.Step=1 AND dcard.ApplyForId=@ApplyForId
                                    INNER JOIN TrnICardRequest req on dcm.RequestId=req.RequestId AND req.StatusId=1
                                    INNER JOIN TrnStepCounter step on req.RequestId=step.RequestId AND step.StepId=12
                                    INNER JOIN BasicDetails basi on req.BasicDetailId=basi.BasicDetailId
                                    INNER JOIN MArmedType marmed on basi.ArmedId=marmed.ArmedId
                                    INNER JOIN MRank ranks on ranks.RankId=basi.RankId
                                    INNER JOIN MapUnit unit on basi.UnitId=unit.UnitMapId
                                    INNER JOIN UserProfile fromUp on dcard.FromUserId=fromUp.UserId
                                    INNER JOIN MRank fromRanks on fromUp.RankId=fromRanks.RankId
                                    INNER JOIN UserProfile toUp on dcard.ToUserId=toUp.UserId
                                    INNER JOIN MRank toRanks on toUp.RankId=toRanks.RankId
                                    INNER JOIN AspNetUsers fromAspUser on dcard.FromAspNetUsersId = fromAspUser.Id
                                    INNER JOIN AspNetUsers toAspUser on dcard.ToAspNetUsersId = toAspUser.Id";
                whereClause = $@"WHERE
                            {unitFilter}
                            AND (@SearchTerm IS NULL OR basi.ServiceNo LIKE @SearchTerm)";
            }
            else if (dTO.Choice == "DispatchToUnit")
            {
                allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["RequestId"] = "req.RequestId",
                    ["ArmedAbbreviation"] = "marmed.Abbreviation",
                    ["ServiceNo"] = "ServiceNo",
                    ["ActionOn"] = "dcard.OutDate"
                };
                selectFields = $@"req.RequestId,ranks.RankAbbreviation as RankName ,basi.FName,basi.LName,basi.ServiceNo,marmed.Abbreviation as ArmedAbbreviation,dcard.OutDate as ActionOn,fromRanks.RankAbbreviation as FromRankName,fromUp.Name as FromName,toRanks.RankAbbreviation as ToRankName,toUp.Name as ToName,fromUp.ArmyNo as FromServiceNo,toUp.ArmyNo as ToServiceNo,fromAspUser.DomainId as FromDID,toAspUser.DomainId as ToDID";
                fromJoinClause = $@"from TrnDispatchCardMapping dcm  --Card Dispatch to Unit
                                    INNER JOIN TrnDispatchCard dcard on dcm.DispatchCardId =dcard.DispatchCardId AND dcard.Step=2 AND dcard.ApplyForId={dTO.ApplyForId}
                                    INNER JOIN TrnICardRequest req on dcm.RequestId=req.RequestId AND req.StatusId=1
                                    INNER JOIN TrnStepCounter step on req.RequestId=step.RequestId AND step.StepId=13
                                    INNER JOIN BasicDetails basi on req.BasicDetailId=basi.BasicDetailId
                                    INNER JOIN MArmedType marmed on basi.ArmedId=marmed.ArmedId
                                    INNER JOIN MRank ranks on ranks.RankId=basi.RankId
                                    INNER JOIN MapUnit unit on basi.UnitId=unit.UnitMapId
                                    INNER JOIN UserProfile fromUp on dcard.FromUserId=fromUp.UserId
                                    INNER JOIN MRank fromRanks on fromUp.RankId=fromRanks.RankId
                                    INNER JOIN UserProfile toUp on dcard.ToUserId=toUp.UserId
                                    INNER JOIN MRank toRanks on toUp.RankId=toRanks.RankId
                                    INNER JOIN AspNetUsers fromAspUser on dcard.FromAspNetUsersId = fromAspUser.Id
                                    INNER JOIN AspNetUsers toAspUser on dcard.ToAspNetUsersId = toAspUser.Id";
                whereClause = $@"WHERE
                            {unitFilter}
                            AND (@SearchTerm IS NULL OR basi.ServiceNo LIKE @SearchTerm)";
            }
            else if (dTO.Choice == "CardInUnit")
            {
                allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["RequestId"] = "req.RequestId",
                    ["ArmedAbbreviation"] = "marmed.Abbreviation",
                    ["ServiceNo"] = "ServiceNo",
                    ["ActionOn"] = "dcard.ReceiptDate"
                };
                selectFields = $@"req.RequestId,ranks.RankAbbreviation as RankName ,basi.FName,basi.LName,basi.ServiceNo,marmed.Abbreviation as ArmedAbbreviation,dcard.ReceiptDate as ActionOn,fromRanks.RankAbbreviation as FromRankName,fromUp.Name as FromName,toRanks.RankAbbreviation as ToRankName,toUp.Name as ToName,fromUp.ArmyNo as FromServiceNo,toUp.ArmyNo as ToServiceNo,fromAspUser.DomainId as FromDID,toAspUser.DomainId as ToDID";
                fromJoinClause = $@"from TrnDispatchCardMapping dcm --Card in Unit
                                    INNER JOIN TrnDispatchCard dcard on dcm.DispatchCardId =dcard.DispatchCardId AND dcard.Step=2 AND dcard.ApplyForId=@ApplyForId
                                    INNER JOIN TrnICardRequest req on dcm.RequestId=req.RequestId AND req.StatusId=1
                                    INNER JOIN TrnStepCounter step on req.RequestId=step.RequestId AND step.StepId=14
                                    INNER JOIN BasicDetails basi on req.BasicDetailId=basi.BasicDetailId
                                    INNER JOIN MArmedType marmed on basi.ArmedId=marmed.ArmedId
                                    INNER JOIN MRank ranks on ranks.RankId=basi.RankId
                                    INNER JOIN MapUnit unit on basi.UnitId=unit.UnitMapId
                                    INNER JOIN UserProfile fromUp on dcard.FromUserId=fromUp.UserId
                                    INNER JOIN MRank fromRanks on fromUp.RankId=fromRanks.RankId
                                    INNER JOIN UserProfile toUp on dcard.ToUserId=toUp.UserId
                                    INNER JOIN MRank toRanks on toUp.RankId=toRanks.RankId
                                    INNER JOIN AspNetUsers fromAspUser on dcard.FromAspNetUsersId = fromAspUser.Id
                                    INNER JOIN AspNetUsers toAspUser on dcard.ToAspNetUsersId = toAspUser.Id";
                whereClause = $@"WHERE
                            {unitFilter}
                            AND (@SearchTerm IS NULL OR basi.ServiceNo LIKE @SearchTerm)";
            }
            else if (dTO.Choice == "CardDistributed")
            {
                allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["RequestId"] = "req.RequestId",
                    ["ArmedAbbreviation"] = "marmed.Abbreviation",
                    ["ServiceNo"] = "ServiceNo",
                    ["ActionOn"] = "dist.DistributedOn"
                };
                selectFields = @"req.RequestId,ranks.RankAbbreviation AS RankName,basi2.FName,basi2.LName,basi2.ServiceNo,marmed.Abbreviation AS ArmedAbbreviation,dist.DistributedOn AS ActionOn,toRanks.RankAbbreviation AS ToRankName,toUp.Name AS ToName,toUp.ArmyNo AS ToServiceNo,toAspUser.DomainId AS ToDID";
                fromJoinClause = $@"        
                                    FROM TrnDistributeCards dist
                                    INNER JOIN TrnStepCounter step ON dist.RequestId = step.RequestId AND step.StepId = 15
                                    INNER JOIN TrnICardRequest req ON step.RequestId = req.RequestId AND req.StatusId = 2
                                    INNER JOIN AFSAC2.dbo.BasicDetails basi2 ON req.BasicDetailId = basi2.BasicDetailId AND basi2.ApplyForId = @ApplyForId
                                    INNER JOIN MArmedType marmed ON marmed.ArmedId = basi2.ArmedId
                                    INNER JOIN MRank ranks ON ranks.RankId = basi2.RankId
                                    INNER JOIN MapUnit unit ON unit.UnitMapId = basi2.UnitId
                                    INNER JOIN UserProfile toUp ON dist.UpdatedbyUserId = toUp.UserId
                                    INNER JOIN MRank toRanks ON toUp.RankId = toRanks.RankId
                                    INNER JOIN AspNetUsers toAspUser ON dist.Updatedby = toAspUser.Id";
                whereClause = $@"WHERE
                            {unitFilter}
                            AND (
	                            @SearchTerm IS NULL
                                OR basi2.ServiceNo LIKE @SearchTerm
                                )";
            }
                try
                {
                    var sortColumn = allowedSortColumns.ContainsKey(dTO.sortColumn ?? "") ? allowedSortColumns[dTO.sortColumn!] : "req.RequestId";

                    var multiQuery = $@"
                        WITH RecordCTE AS (
                            select  Count(*) OVER () as TotalFilteredRecords,ROW_NUMBER() OVER (ORDER BY {sortColumn} {sortOrder}) AS RowNum, {selectFields} {fromJoinClause} {whereClause}
                        )
                        SELECT * FROM RecordCTE WHERE RowNum BETWEEN @Offset AND @Limit;";

                    using (var connection = _contextDP.CreateConnection())
                    {
                        var searchTerm = string.IsNullOrEmpty(dTO.searchValue) ? null : $"%{dTO.searchValue}%";

                        var parameters = new DynamicParameters();
                        parameters.Add("@ApplyForId", dTO.ApplyForId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@UnitMapId", dTO.UnitMapId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@UnitType", dTO.UnitType, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@ComdId", dTO.ComdId, DbType.Byte, ParameterDirection.Input);
                        parameters.Add("@CorpsId", dTO.CorpsId, DbType.Byte, ParameterDirection.Input);
                        parameters.Add("@DivId", dTO.DivId, DbType.Byte, ParameterDirection.Input);
                        parameters.Add("@BdeId", dTO.BdeId, DbType.Byte, ParameterDirection.Input);
                        parameters.Add("@FmnBranchID", dTO.FmnBranchID, DbType.Byte, ParameterDirection.Input);
                        parameters.Add("@PsoId", dTO.PsoId, DbType.Byte, ParameterDirection.Input);
                        parameters.Add("@SubDteId", dTO.SubDteId, DbType.Byte, ParameterDirection.Input);
                        parameters.Add("@Offset", dTO.Start + 1, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@Limit", (dTO.Start + dTO.Length), DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@SearchTerm", searchTerm, DbType.String, ParameterDirection.Input);

                        var ret = await connection.QueryMultipleAsync(multiQuery, parameters);
                        var records = (await ret.ReadAsync<DTOReportCardResponse>()).ToList();
                        var totalFilteredRecords = records?.FirstOrDefault()?.TotalFilteredRecords;

                        var responseData = new DTODataTablesResponse<DTOReportCardResponse>
                        {
                            draw = dTO.Draw,
                            recordsTotal = totalFilteredRecords.GetValueOrDefault(),
                            recordsFiltered = totalFilteredRecords.GetValueOrDefault(),
                            data = records,
                        };
                        return responseData;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(1001, ex, "ReportReturnDB->GetReportCardData");
                    List<DTOReportCardResponse> dTOUserRegnResponses = new List<DTOReportCardResponse>();
                    var responseData = new DTODataTablesResponse<DTOReportCardResponse>
                    {
                        draw = dTO.Draw,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = dTOUserRegnResponses
                    };
                    return responseData;
                }
        }
    }
}