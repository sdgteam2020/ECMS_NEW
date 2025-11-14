using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
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
            #region Old Code by Kapoor Sir
            //string query = " SELECT fwd.StepId,fwd.IsComplete,fwd.TrnFwdId into tempTrnFwds  from TrnFwds fwd" +
            //               "  inner join TrnStepCounter step on fwd.RequestId=step.RequestId AND fwd.StepId=fwd.StepId and step.ApplyForId=@ApplyForId " +
            //               "  left join TrnICardRequest req on step.RequestId=req.RequestId and req.StatusId=1  " +
            //               "  left join  BasicDetails basi on req.BasicDetailId=basi.BasicDetailId  " +
            //               "  left join MapUnit unit on basi.UnitId=unit.UnitMapId " +
            //               " where unit.ComdId=ISNULL(@ComdId,unit.ComdId) " +
            //               " and unit.CorpsId=ISNULL(@CorpsId,unit.CorpsId)" +
            //               " and unit.DivId=ISNULL(@DivId,unit.DivId)" +
            //               " and unit.BdeId=ISNULL(@BdeId,unit.BdeId)" +
            //               //" and unit.FmnBranchID=ISNULL(@FmnBranchID,unit.FmnBranchID)" +
            //               //" and unit.PsoId=ISNULL(@PsoId,unit.PsoId)" +
            //               //" and unit.SubDteId=ISNULL(@SubDteId,unit.SubDteId)" +
            //               " and unit.UnitMapId=ISNULL(@UnitMapId,unit.UnitMapId) " +
            //               " group by fwd.StepId,fwd.IsComplete,fwd.TrnFwdId " +
            //               " create table #Temp(StepId int, IsApprove int, Total int)" +
            //               " SELECT * into tempstep FROM MStepCounterStep WHERE IsDashboard=1" +
            //               " declare @Total int=0" +
            //               " declare @IsApprove int=0" +
            //               " declare @StepId int=0" +
            //               " while ((select COUNT(*) total from tempstep) > 0)" +
            //               " begin" +
            //               " set @Total=0" +
            //               " set @IsApprove=0" +
            //               " set @StepId=0" +
            //               " Select Top 1 @StepId=StepId from tempstep" +
            //               " if exists(select * from tempstep where StepId=@StepId and @StepId=1)" +
            //               " begin" +
            //               " SELECT @Total=COUNT(*) from TrnStepCounter step" +
            //               "  left join TrnICardRequest req on step.RequestId=req.RequestId and req.StatusId=1  " +
            //               "  left join  BasicDetails basi on req.BasicDetailId=basi.BasicDetailId  " +
            //               "  left join MapUnit unit on basi.UnitId=unit.UnitMapId " +
            //               "  where unit.ComdId=ISNULL(@ComdId,unit.ComdId) " +
            //               "  and unit.CorpsId=ISNULL(@CorpsId,unit.CorpsId)" +
            //               "  and unit.DivId=ISNULL(@DivId,unit.DivId)" +
            //               "  and unit.BdeId=ISNULL(@BdeId,unit.BdeId)" +
            //               //"  and unit.FmnBranchID=ISNULL(@FmnBranchID,unit.FmnBranchID)" +
            //               //"  and unit.PsoId=ISNULL(@PsoId,unit.PsoId)" +
            //               //"  and unit.SubDteId=ISNULL(@SubDteId,unit.SubDteId)" +
            //               "  and unit.UnitMapId=ISNULL(@UnitMapId,unit.UnitMapId) " +
            //               "  and step.ApplyForId=@ApplyForId and step.StepId=@StepId" +

            //               " INSERT INTO #Temp(StepId,IsApprove,Total)VALUES(@StepId,@IsApprove,@Total)" +
            //               " end" +
            //               " else if exists(select * from tempstep where StepId=@StepId and @StepId in (2,3,4) )" +
            //               " begin " +
            //               "  select @Total=COUNT(*) from tempTrnFwds where tempTrnFwds.StepId=@StepId and IsComplete=1" +
            //               "  INSERT INTO #Temp(StepId,IsApprove,Total)VALUES(@StepId,1,@Total)" +
            //               "  select @Total=COUNT(*) from tempTrnFwds where tempTrnFwds.StepId=@StepId and IsComplete=0" +
            //               "  INSERT INTO #Temp(StepId,IsApprove,Total)VALUES(@StepId,0,@Total)" +
            //               " End" +
            //               " else " +
            //               " begin" +
            //               " INSERT INTO #Temp(StepId,IsApprove,Total)VALUES(@StepId,0,(select COUNT(*) from tempTrnFwds where tempTrnFwds.StepId=@StepId and IsComplete=0))" +
            //               " End" +
            //               " delete from tempstep where StepId=@StepId" +
            //               " End" +
            //               " select a.StepId,a.IsApprove,a.Total,b.Name,b.TypeId,b.OrderBy  from #Temp a inner join MStepCounterStep b on a.StepId=b.StepId" +
            //               " order by b.TypeId,a.StepId,b.OrderBy" +
            //               " drop table tempstep" +
            //               " drop table #Temp" +
            //               " drop table tempTrnFwds";


            ////string query = " select Count(fwd.RequestId) Total,Mstep.Name,Mstep.StepId,Mstep.TypeId," +
            ////               " ISNULL(fwd.IsComplete,0) IsComplete,ISNULL(fwd.FwdStatusId,0) FwdStatusId from MStepCounterStep Mstep" +
            ////               " left join TrnFwds fwd on Mstep.StepId=fwd.StepId" +
            ////               " left join TrnStepCounter step on Mstep.StepId=step.StepId and step.ApplyForId=@ApplyForId" +
            ////               " left join TrnICardRequest req on step.RequestId=req.RequestId and req.StatusId=1 " +
            ////               " left join  BasicDetails basi on req.BasicDetailId=basi.BasicDetailId " +
            ////               " left join MapUnit unit on basi.UnitId=unit.UnitMapId" +
            ////               " and unit.ComdId=ISNULL(@ComdId,unit.ComdId) " +
            ////               " and unit.CorpsId=ISNULL(@CorpsId,unit.CorpsId)" +
            ////               " and unit.DivId=ISNULL(@DivId,unit.DivId)" +
            ////               " and unit.BdeId=ISNULL(@BdeId,unit.BdeId)" +
            ////               " and unit.BdeId=ISNULL(@FmnBranchID,unit.FmnBranchID)" +
            ////               " and unit.BdeId=ISNULL(@PsoId,unit.PsoId)" +
            ////               " and unit.BdeId=ISNULL(@SubDteId,unit.SubDteId)" +
            ////               " and unit.UnitMapId=ISNULL(@UnitMapId,unit.UnitMapId)" +
            ////               " where Mstep.IsDashboard=1" +
            ////               " group by Mstep.Name,Mstep.StepId,Mstep.TypeId,fwd.IsComplete,fwd.FwdStatusId" +
            ////               " order by Mstep.TypeId,Mstep.StepId,fwd.IsComplete,fwd.FwdStatusId";
            //try
            //{
            //    using (var connection = _contextDP.CreateConnection())
            //    {
            //        var ret = await connection.QueryAsync<DTOReportReturnCount>(query, new { ApplyForId, Data.ComdId, Data.CorpsId, Data.DivId, Data.BdeId, Data.FmnBranchID, Data.PsoId, Data.SubDteId, Data.UnitMapId });
            //        return ret.ToList();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(1001, ex, "ReportReturnDB->GetMstepCount");
            //    return new List<DTOReportReturnCount>();
            //}
            #endregion
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
                            unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                            AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                            AND unit.DivId = ISNULL(@DivId, unit.DivId)
                            AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                            AND unit.UnitMapId = ISNULL(@UnitMapId, unit.UnitMapId)
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
                                    unit.ComdId = ISNULL(@ComdId, unit.ComdId)
                                    AND unit.CorpsId = ISNULL(@CorpsId, unit.CorpsId)
                                    AND unit.DivId = ISNULL(@DivId, unit.DivId)
                                    AND unit.BdeId = ISNULL(@BdeId, unit.BdeId)
                                    AND unit.UnitMapId = ISNULL(@UnitMapId, unit.UnitMapId)
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
        /// Retrieves Mstep count specifically for approved and rejected status.
        /// </summary>
        /// <param name="Data">The data that contains the hierarchy request parameters.</param>
        /// <param name="ApplyForId">The ApplyForId used to filter the MStepCount data.</param>
        /// <returns>A list of DTOReportReturnCount objects representing the approved and rejected Mstep counts.</returns>
        /// <exception cref="Exception">Throws exception if there is an error during the database operation.</exception>
        public async Task<List<DTOReportReturnCount>> GetMstepCountApprovedReject(DTOMHierarchyRequest Data, int ApplyForId)
        {
            try
            {
                List<DTOReportReturnCount> lst = new List<DTOReportReturnCount>();
                string query = " SELECT COUNT(*) Total,Mfsts.FwdStatusId StepId,fwd.TypeId FROM TrnFwds fwd" +
                               " inner join MTrnFwdStatus Mfsts  on Mfsts.FwdStatusId=fwd.FwdStatusId " +
                               " inner join MFwdType Mftype on Mftype.TypeId=fwd.TypeId" +
                               " inner join TrnICardRequest req on fwd.RequestId=req.RequestId and req.StatusId=1" +
                               " inner join  BasicDetails basi on req.BasicDetailId=basi.BasicDetailId " +
                               " left join MapUnit unit on basi.UnitId=unit.UnitMapId " +
                               " and unit.ComdId=ISNULL(@ComdId,unit.ComdId) " +
                               " and unit.CorpsId=ISNULL(@CorpsId,unit.CorpsId)" +
                               " and unit.DivId=ISNULL(@DivId,unit.DivId)" +
                               " and unit.BdeId=ISNULL(@BdeId,unit.BdeId)" +
                               //" and unit.FmnBranchID=ISNULL(@FmnBranchID,unit.FmnBranchID)" +
                               //" and unit.PsoId=ISNULL(@PsoId,unit.PsoId)" +
                               //" and unit.SubDteId=ISNULL(@SubDteId,unit.SubDteId)" +
                               " and unit.UnitMapId=ISNULL(@UnitMapId,unit.UnitMapId)" +
                               " where basi.ApplyForId=@ApplyForId and  fwd.FwdStatusId in (3) " +
                               " group by Mfsts.FwdStatusId,fwd.TypeId";


                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTOReportReturnCount>(query, new { Data.ComdId, Data.CorpsId, Data.DivId, Data.BdeId, Data.FmnBranchID, Data.PsoId, Data.SubDteId, Data.UnitMapId, ApplyForId });
                    lst = ret.ToList();
                }
                string query1 = " SELECT COUNT(*) Total,Mfsts.FwdStatusId StepId,fwd.TypeId FROM TrnFwds fwd" +
                              " inner join MTrnFwdStatus Mfsts  on Mfsts.FwdStatusId=fwd.FwdStatusId " +
                              " inner join MFwdType Mftype on Mftype.TypeId=fwd.TypeId" +
                              " inner join TrnICardRequest req on fwd.RequestId=req.RequestId and req.StatusId=1" +
                              " inner join  BasicDetails basi on req.BasicDetailId=basi.BasicDetailId " +
                              " left join MapUnit unit on basi.UnitId=unit.UnitMapId " +
                              " and unit.ComdId=ISNULL(@ComdId,unit.ComdId) " +
                               " and unit.CorpsId=ISNULL(@CorpsId,unit.CorpsId)" +
                               " and unit.DivId=ISNULL(@DivId,unit.DivId)" +
                               " and unit.BdeId=ISNULL(@BdeId,unit.BdeId)" +
                               //" and unit.FmnBranchID=ISNULL(@FmnBranchID,unit.FmnBranchID)" +
                               //" and unit.PsoId=ISNULL(@PsoId,unit.PsoId)" +
                               //" and unit.SubDteId=ISNULL(@SubDteId,unit.SubDteId)" +
                               " and unit.UnitMapId=ISNULL(@UnitMapId,unit.UnitMapId)" +
                              " where basi.ApplyForId=@ApplyForId and  fwd.FwdStatusId in (2)  and IsComplete=1 " +
                              " group by Mfsts.FwdStatusId,fwd.TypeId";


                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTOReportReturnCount>(query1, new { Data.ComdId, Data.CorpsId, Data.DivId, Data.BdeId, Data.FmnBranchID, Data.PsoId, Data.SubDteId, Data.UnitMapId, ApplyForId });
                    lst.AddRange(ret.ToList());
                }
                return lst;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "ReportReturnDB->GetRecordJco");
                return new List<DTOReportReturnCount>();
            }
        }

        /// <summary>
        /// Fetches the count of records for the "Mstep" process.
        /// </summary>
        /// <param name="Data">The input data for hierarchical request.</param>
        /// <param name="ApplyForId">The ApplyForId for filtering the records.</param>
        /// <returns>Returns a list of DTOReportReturnCount based on the count for Mstep.</returns>
        /// <exception cref="NotImplementedException">Thrown if the method is not yet implemented.</exception>
        public Task<List<DTOReportReturnCount>> GetMstepCountApprovedRejectJco(DTOMHierarchyRequest Data, int ApplyForId)
        {
            throw new NotImplementedException();
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
            #region Old code
            //string query = " select COUNT(req.RequestId) Total, fwdsts.Name,fwdsts.FwdStatusId,mrec.RecordOfficeId, 3 GroupId from MTrnFwdStatus fwdsts" +
            //   " inner join TrnFwds fwd on fwdsts.FwdStatusId=fwd.FwdStatusId  " +
            //   " inner join TrnICardRequest req on fwd.RequestId=req.RequestId and req.StatusId=1 " +
            //   " inner join TrnStepCounter step on step.ApplyForId=1 and req.RequestId=step.RequestId" +
            //   " inner join TrnDomainMapping map on fwd.ToAspNetUsersId=map.AspNetUsersId  " +
            //   " inner join OROMapping mrec on map.Id=mrec.TDMId " +
            //   " inner join  BasicDetails basi on req.BasicDetailId=basi.BasicDetailId " +
            //   " left join MapUnit unit on basi.UnitId=unit.UnitMapId " +
            //   " where unit.ComdId=ISNULL(@ComdId,unit.ComdId) " +
            //   " and unit.CorpsId=ISNULL(@CorpsId,unit.CorpsId)" +
            //   " and unit.DivId=ISNULL(@DivId,unit.DivId)" +
            //   " and unit.BdeId=ISNULL(@BdeId,unit.BdeId)" +
            //   " and unit.UnitMapId=ISNULL(@UnitMapId,unit.UnitMapId)" +
            //   " group by fwdsts.Name,fwdsts.FwdStatusId,mrec.RecordOfficeId";
            //try
            //{
            //    using (var connection = _contextDP.CreateConnection())
            //    {
            //        var ret = await connection.QueryAsync<DTOReportReturnCount>(query, new { Data.ComdId, Data.CorpsId, Data.DivId, Data.BdeId, Data.FmnBranchID, Data.PsoId, Data.SubDteId, Data.UnitMapId });
            //        return ret.ToList();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(1001, ex, "ReportReturnDB->GetRecordJco");
            //    return new List<DTOReportReturnCount>();
            //}
            #endregion

            string query = @"select COUNT(req.RequestId) Total, fwdsts.Name,fwdsts.FwdStatusId,mrec.RecordOfficeId, 3 GroupId from MTrnFwdStatus fwdsts
                            inner join TrnFwds fwd on fwdsts.FwdStatusId=fwd.FwdStatusId and fwd.StepId in (3,8)
                            inner join TrnICardRequest req on fwd.RequestId=req.RequestId and req.StatusId=1 
                            inner join OROMapping mrec on req.RecordOfficeId=mrec.RecordOfficeId 
                            inner join  BasicDetails basi on req.BasicDetailId=basi.BasicDetailId and basi.ApplyForId=1
                            left join MapUnit unit on basi.UnitId=unit.UnitMapId 
                            where unit.ComdId=ISNULL(@ComdId,unit.ComdId) 
                            and unit.CorpsId=ISNULL(@CorpsId,unit.CorpsId)
                            and unit.DivId=ISNULL(@DivId,unit.DivId)
                            and unit.BdeId=ISNULL(@BdeId,unit.BdeId)
                            and unit.UnitMapId=ISNULL(@UnitMapId,unit.UnitMapId)
                            group by fwdsts.Name,fwdsts.FwdStatusId,mrec.RecordOfficeId";
            try 
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTOReportReturnCount>(query, new { Data.ComdId, Data.CorpsId, Data.DivId, Data.BdeId, Data.FmnBranchID, Data.PsoId, Data.SubDteId, Data.UnitMapId });
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
            #region Old Code
            //string query = " select count(req.RequestId) Total ,recf.RecordOfficeId,recf.Name,step.StepId from MRecordOffice recf" +
            //   " left join TrnDomainMapping map on recf.TDMId=map.Id" +
            //   " left join TrnFwds fwd on map.AspNetUsersId=fwd.ToAspNetUsersId and fwd.IsComplete=@IsComplete and fwd.StepId=3" +
            //   " left join TrnICardRequest req on fwd.RequestId=req.RequestId and req.StatusId=1  " +
            //   " left join TrnStepCounter step on req.RequestId=step.RequestId " +
            //   " left join MRecordOffice mrec on map.Id=mrec.TDMId and mrec.ArmedId!=@ArmedIdForORO " +
            //   " left join BasicDetails basi on req.BasicDetailId=basi.BasicDetailId " +
            //   " left join MapUnit unit on basi.UnitId=unit.UnitMapId" +
            // " where unit.ComdId=ISNULL(@ComdId,unit.ComdId) " +
            //   " and unit.CorpsId=ISNULL(@CorpsId,unit.CorpsId)" +
            //   " and unit.DivId=ISNULL(@DivId,unit.DivId)" +
            //   " and unit.BdeId=ISNULL(@BdeId,unit.BdeId)" +
            //   " and unit.UnitMapId=ISNULL(@UnitMapId,unit.UnitMapId)" +
            //   " and recf.ArmedId!=@ArmedIdForORO   group by recf.RecordOfficeId,recf.Name,step.StepId";
            //try
            //{
            //    using (var connection = _contextDP.CreateConnection())
            //    {
            //        var ret = await connection.QueryAsync<DTOReportReturnCount>(query, new { IsComplete, Data.ComdId, Data.CorpsId, Data.DivId, Data.BdeId, Data.FmnBranchID, Data.PsoId, Data.SubDteId, Data.UnitMapId, ArmedIdForORO });
            //        return ret.ToList();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(1001, ex, "ReportReturnDB->GetReportForm11");
            //    return new List<DTOReportReturnCount>();
            //}
            #endregion
            string query = @"select count(req.RequestId) Total ,recf.RecordOfficeId,recf.Name,step.StepId from MRecordOffice recf
                            inner join TrnICardRequest req on recf.RecordOfficeId=req.RecordOfficeId and req.StatusId=1  
                            inner join TrnFwds fwd on req.RequestId=fwd.RequestId and fwd.IsComplete=@IsComplete and fwd.StepId=3
                            inner join TrnStepCounter step on req.RequestId=step.RequestId 
                            inner join BasicDetails basi on req.BasicDetailId=basi.BasicDetailId and basi.ApplyForId=2
                            left join MapUnit unit on basi.UnitId=unit.UnitMapId
                            where unit.ComdId=ISNULL(@ComdId,unit.ComdId) 
                            and unit.CorpsId=ISNULL(@CorpsId,unit.CorpsId)
                            and unit.DivId=ISNULL(@DivId,unit.DivId)
                            and unit.BdeId=ISNULL(@BdeId,unit.BdeId)
                            and unit.UnitMapId=ISNULL(@UnitMapId,unit.UnitMapId)
                            and recf.ArmedId!=@ArmedIdForORO group by recf.RecordOfficeId,recf.Name,step.StepId";
            try 
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@IsComplete", IsComplete, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@ArmedIdForORO", ArmedIdForORO, DbType.Int16, ParameterDirection.Input);
                    parameters.Add("@UnitMapId", Data.UnitMapId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@ComdId", Data.ComdId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@CorpsId", Data.CorpsId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@DivId", Data.DivId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@BdeId", Data.BdeId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@FmnBranchID", Data.FmnBranchID, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@PsoId", Data.PsoId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@SubDteId", Data.SubDteId, DbType.Byte, ParameterDirection.Input);

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
                    
                    wherequery = @"WHERE unit.ComdId=ISNULL(@ComdId,unit.ComdId) 
                                and unit.CorpsId=ISNULL(@CorpsId,unit.CorpsId)
                                and unit.DivId=ISNULL(@DivId,unit.DivId)
                                and unit.BdeId=ISNULL(@BdeId,unit.BdeId)
                                and unit.UnitMapId=ISNULL(@UnitMapId,unit.UnitMapId)
                                and step.ApplyForId=2 and fwd.IsComplete=0 and fwd.StepId=3 and mrec.RecordOfficeId=@ApplyForId and ServiceNo like '%' + @SearchTerm + '%' ";
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
                    
                    wherequery = @"WHERE unit.ComdId=ISNULL(@ComdId,unit.ComdId) 
                                and unit.CorpsId=ISNULL(@CorpsId,unit.CorpsId)
                                and unit.DivId=ISNULL(@DivId,unit.DivId)
                                and unit.BdeId=ISNULL(@BdeId,unit.BdeId)
                                and unit.UnitMapId=ISNULL(@UnitMapId,unit.UnitMapId)
                                and step.ApplyForId=@ApplyForId and step.StepId=@StepId and fwd.StepId=@StepId and ServiceNo like '%' + @SearchTerm + '%'";

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
                        
                        wherequery = @"where unit.ComdId=ISNULL(@ComdId,unit.ComdId) 
                                    and unit.CorpsId=ISNULL(@CorpsId,unit.CorpsId)
                                    and unit.DivId=ISNULL(@DivId,unit.DivId)
                                    and unit.BdeId=ISNULL(@BdeId,unit.BdeId)
                                    and unit.UnitMapId=ISNULL(@UnitMapId,unit.UnitMapId)
                                    and step.ApplyForId=@ApplyForId and ServiceNo like '%' + @SearchTerm + '%'";
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
                        
                        wherequery = @"where unit.ComdId=ISNULL(@ComdId,unit.ComdId) 
                                    and unit.CorpsId=ISNULL(@CorpsId,unit.CorpsId)
                                    and unit.DivId=ISNULL(@DivId,unit.DivId)
                                    and unit.BdeId=ISNULL(@BdeId,unit.BdeId)
                                    and unit.UnitMapId=ISNULL(@UnitMapId,unit.UnitMapId)
                                    and step.ApplyForId=@ApplyForId and fwd.StepId=@StepId and ServiceNo like '%' + @SearchTerm + '%'";
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

                    wherequery = @"where unit.ComdId=ISNULL(@ComdId,unit.ComdId) 
                                and unit.CorpsId=ISNULL(@CorpsId,unit.CorpsId)
                                and unit.DivId=ISNULL(@DivId,unit.DivId)
                                and unit.BdeId=ISNULL(@BdeId,unit.BdeId)
                                and unit.UnitMapId=ISNULL(@UnitMapId,unit.UnitMapId)
                                and mrec.RecordOfficeId=@ApplyForId and ServiceNo like '%' + @SearchTerm + '%'";

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
                    draw = 0,
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

            var sortOrder = dTO.sortDirection;
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
                            INNER JOIN TrnICardRequest req on step.RequestId=req.RequestId and req.StatusId=1
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
                            AND ServiceNo LIKE '%' + @SearchTerm + '%'";
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
                query = @"appl.Name ApplyFor,marmed.Abbreviation as ArmedAbbreviation,regi.Abbreviation RegimentalName,mcat.Name FaultyStage,req.RequestId,bas.ServiceNo,ranks.RankAbbreviation RankName,bas.FName,bas.LName,bas.NameAsPerRecord,Muni.Abbreviation UnitAbbreviation,
                            faulty.UpdatedOn,faulty.FromRemark,faulty.ToRemark,
                            (select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(faulty.RemarksIds,','))) RemarksNameList
                            from TrnFaultyCard faulty
                            inner join MCategory mcat on mcat.CategoryId = faulty.CategoryId
                            inner join TrnICardRequest req on req.RequestId = faulty.RequestId
                            inner join TrnDomainMapping tdm on tdm.Id=req.TrnDomainMappingId
                            inner join BasicDetails bas on bas.BasicDetailId=req.BasicDetailId
							INNER JOIN MArmedType marmed on bas.ArmedId=marmed.ArmedId
                            inner join MRank ranks on ranks.RankId=bas.RankId
                            inner join MapUnit unit on unit.UnitMapId=bas.UnitId
                            inner join MUnit Muni on Muni.UnitId=unit.UnitId
                            inner join MApplyFor appl on appl.ApplyForId=bas.ApplyForId
                            left join MRegimental regi on regi.RegId=bas.RegimentalId ";
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
                            AND ServiceNo LIKE '%' + @SearchTerm + '%'";
            }
            else if (dTO.Choice == "LostCase")
            {
                allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["ServiceNo"] = "bas.ServiceNo",
                    ["RequestId"] = "req.RequestId",
                    ["ArmedAbbreviation"] = "marmed.Abbreviation",
                    ["ApplyFor"] = "appl.Name",
                    ["IsFIRLogged"] = "lost.IsFIRLogged",
                    ["LostOn"] = "lost.LostOn",
                    ["UpdatedOn"] = "lost.UpdatedOn"
                };
                query = @"appl.Name ApplyFor,marmed.Abbreviation as ArmedAbbreviation,req.RequestId,bas.ServiceNo,ranks.RankAbbreviation RankName,bas.FName,bas.LName,bas.NameAsPerRecord,Muni.Abbreviation UnitAbbreviation,
		                    lost.UpdatedOn,lost.Remark as FromRemark,lost.LostOn,regi.Abbreviation RegimentalName,lost.IsFIRLogged,lost.SupportDocName
                            from TrnLostCards lost
                            inner join TrnICardRequest req on req.RequestId = lost.RequestId
                            inner join TrnDomainMapping tdm on tdm.Id=req.TrnDomainMappingId
                            inner join BasicDetails bas on bas.BasicDetailId=req.BasicDetailId
		                    INNER JOIN MArmedType marmed on bas.ArmedId=marmed.ArmedId
                            inner join MRank ranks on ranks.RankId=bas.RankId
                            inner join MapUnit unit on unit.UnitMapId=bas.UnitId
                            inner join MUnit Muni on Muni.UnitId=unit.UnitId
                            inner join MApplyFor appl on appl.ApplyForId=bas.ApplyForId
                            left join MRegimental regi on regi.RegId=bas.RegimentalId ";
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
                            AND ServiceNo LIKE '%' + @SearchTerm + '%'";
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
                            INNER JOIN TrnICardRequest req on step.RequestId=req.RequestId and req.StatusId=1
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
                            AND ServiceNo LIKE '%' + @SearchTerm + '%'
                            AND YEAR(basi.UpdatedOn) = RIGHT(@MonthYear, 4)
							AND MONTH(basi.UpdatedOn) = LEFT(@MonthYear, 2)";
            }
            try
            {
                var sortColumn = allowedSortColumns.ContainsKey(dTO.sortColumn ?? "")
                ? allowedSortColumns[dTO.sortColumn!]
                : "ServiceNo";
                var multiQuery = query = $@"
                        WITH RecordCTE AS (
                            select  Count(*) OVER () as TotalFilteredRecords,ROW_NUMBER() OVER (ORDER BY {sortColumn} {sortOrder}) AS RowNum, {query} {wherequery}
                        )
                        SELECT * FROM RecordCTE WHERE RowNum BETWEEN @Offset AND @Limit;";

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
                    parameters.Add("@MonthYear", dTO.MonthYear, DbType.String, ParameterDirection.Input);
                    parameters.Add("@Offset", dTO.Start + 1, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Limit", (dTO.Start + dTO.Length) , DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@SearchTerm", dTO.searchValue, DbType.String, ParameterDirection.Input);

                    var ret = await connection.QueryMultipleAsync(query, parameters);
                    var records = (await ret.ReadAsync<DTOReportResponse>()).ToList();
                    var totalFilteredRecords = records?.FirstOrDefault()?.TotalFilteredRecords;

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
                    draw = 0,
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

                            Select @TotRequisition=COUNT(distinct req.RequestId) from TrnICardRequest req
                            INNER JOIN  BasicDetails basi on req.BasicDetailId=basi.BasicDetailId
                            INNER JOIN MapUnit unit on basi.UnitId=unit.UnitMapId
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
                            AND req.StatusId=1

                            SELECT @TotLostCases=COUNT(distinct req.RequestId) from TrnLostCards lost
                            inner join TrnICardRequest req on req.RequestId = lost.RequestId
                            inner join BasicDetails bas on bas.BasicDetailId=req.BasicDetailId
                            inner join MapUnit unit on unit.UnitMapId=bas.UnitId
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
                            AND unit.UnitType =@UnitType
                            AND unit.UnitMapId = ISNULL(@UnitMapId, unit.UnitMapId)

                            Select @TotMonthlyProcessed=COUNT(distinct req.RequestId) from TrnICardRequest req
                                    INNER JOIN  BasicDetails basi on req.BasicDetailId=basi.BasicDetailId
                                    INNER JOIN MapUnit unit on basi.UnitId=unit.UnitMapId
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
                            AND req.StatusId=1
                            AND unit.UnitMapId = ISNULL(@UnitMapId, unit.UnitMapId)
                            AND unit.UnitType =@UnitType
                            AND YEAR(basi.UpdatedOn) = YEAR(GETDATE())
                            AND MONTH(basi.UpdatedOn) = MONTH(GETDATE())

                            SELECT @TotNonFunctionalCard=COUNT(distinct faulty.TrnFaultyCardId) from TrnFaultyCard faulty
                            inner join TrnICardRequest req on req.RequestId = faulty.RequestId
                            inner join BasicDetails bas on bas.BasicDetailId=req.BasicDetailId
                            inner join MapUnit unit on unit.UnitMapId=bas.UnitId
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
                            AND unit.UnitType =@UnitType
                            AND unit.UnitMapId = ISNULL(@UnitMapId, unit.UnitMapId)

                            select @TotRequisition TotRequisition,@TotLostCases TotLostCases,@TotMonthlyProcessed TotMonthlyProcessed,@TotNonFunctionalCard TotNonFunctionalCard";

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
            string query = " select " +
                           "   req.RequestId, " +
                           "   basi.FName,basi.LName, " +
                           "   ServiceNo, " +
                           "   DOB, " +
                           "   ranks.RankAbbreviation RankName " +
                           " from " +
                           "   MStepCounterStep Mstep " +
                           "   inner join TrnStepCounter step on Mstep.StepId = step.StepId " +
                           "   inner join TrnICardRequest req on step.RequestId = req.RequestId   and req.StatusId = 1 " +
                           "   inner join BasicDetails basi on req.BasicDetailId = basi.BasicDetailId " +
                           "   left join TrnFwds fwd on req.RequestId = fwd.RequestId " +
                           "   left join MTrnFwdStatus fwdsts on fwd.FwdStatusId = fwdsts.FwdStatusId " +
                           "   left join MRank ranks on ranks.RankId = basi.RankId " +
                           "   left join MapUnit unit on basi.UnitId = unit.UnitMapId " +
                           " where unit.ComdId=ISNULL(@ComdId,unit.ComdId) " +
                           " and unit.CorpsId=ISNULL(@CorpsId,unit.CorpsId)" +
                           " and unit.DivId=ISNULL(@DivId,unit.DivId)" +
                           " and unit.BdeId=ISNULL(@BdeId,unit.BdeId)" +
                           //" and unit.FmnBranchID=ISNULL(@FmnBranchID,unit.FmnBranchID)" +
                           //" and unit.PsoId=ISNULL(@PsoId,unit.PsoId)" +
                           //" and unit.SubDteId=ISNULL(@SubDteId,unit.SubDteId)" +
                           " and unit.UnitMapId=ISNULL(@UnitMapId,unit.UnitMapId)";
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
                            ;WITH CTE AS (
                                SELECT 
                                    stcount.RequestId,
                                    basi.ApplyForId,
                                    stcount.StepId,
                                    req.StatusId,
                                    unit.UnitMapId
                                FROM TrnStepCounter stcount
                                INNER JOIN TrnICardRequest req ON stcount.RequestId = req.RequestId
                                INNER JOIN BasicDetails basi ON req.BasicDetailId = basi.BasicDetailId
                                INNER JOIN MapUnit unit ON basi.UnitId = unit.UnitMapId
                                WHERE unit.UnitMapId = ISNULL(@UnitMapId, unit.UnitMapId)
                                    AND unit.UnitType = @UnitType
                                    AND (
                                        -- Unit Type Conditions
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
                            )
                            -- Calculation for each type of count
                            SELECT 
                                SUM(CASE WHEN ApplyForId = 1 AND StepId = 5 AND StatusId = 1 THEN 1 ELSE 0 END) AS TotExported_Officer,
                                SUM(CASE WHEN ApplyForId = 2 AND StepId = 5 AND StatusId = 1 THEN 1 ELSE 0 END) AS TotExported_OR,
                                SUM(CASE WHEN ApplyForId = 1 AND StepId = 6 AND StatusId = 1 THEN 1 ELSE 0 END) AS TotPrinted_Officer,
                                SUM(CASE WHEN ApplyForId = 2 AND StepId = 6 AND StatusId = 1 THEN 1 ELSE 0 END) AS TotPrinted_OR,
	                            SUM(CASE WHEN ApplyForId = 1 AND StepId = 11 AND StatusId = 1 THEN 1 ELSE 0 END) AS TotDispatchToORO,
                                SUM(CASE WHEN ApplyForId = 2 AND StepId = 11 AND StatusId = 1 THEN 1 ELSE 0 END) AS TotDispatchToRegt,
	                            SUM(CASE WHEN ApplyForId = 1 AND StepId = 12 AND StatusId = 1 THEN 1 ELSE 0 END) AS TotCardInORO,
                                SUM(CASE WHEN ApplyForId = 2 AND StepId = 12 AND StatusId = 1 THEN 1 ELSE 0 END) AS TotCardInRegt,
	                            SUM(CASE WHEN ApplyForId = 1 AND StepId = 13 AND StatusId = 1 THEN 1 ELSE 0 END) AS TotDispatchToUnit_Officer,
                                SUM(CASE WHEN ApplyForId = 2 AND StepId = 13 AND StatusId = 1 THEN 1 ELSE 0 END) AS TotDispatchToUnit_OR,
	                            SUM(CASE WHEN ApplyForId = 1 AND StepId = 14 AND StatusId = 1 THEN 1 ELSE 0 END) AS TotCardInUnit_Officer,
                                SUM(CASE WHEN ApplyForId = 2 AND StepId = 14 AND StatusId = 1 THEN 1 ELSE 0 END) AS TotCardInUnit_OR,
	                            SUM(CASE WHEN ApplyForId = 1 AND StepId = 15 AND StatusId = 2 THEN 1 ELSE 0 END) AS TotDistributed_Officer,
                                SUM(CASE WHEN ApplyForId = 2 AND StepId = 15 AND StatusId = 2 THEN 1 ELSE 0 END) AS TotDistributed_OR
                            FROM CTE;";

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

            var sortOrder = dTO.sortDirection;
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
                                    INNER JOIN TrnICardRequest req on step.RequestId=req.RequestId
                                    INNER JOIN BasicDetails basi on req.BasicDetailId=basi.BasicDetailId
                                    INNER JOIN MArmedType marmed on basi.ArmedId=marmed.ArmedId
                                    INNER JOIN MRank ranks on ranks.RankId=basi.RankId
                                    INNER JOIN MapUnit unit on basi.UnitId=unit.UnitMapId";
                whereClause = $@"WHERE
                            {unitFilter}
                            AND req.StatusId=1
                            AND step.StepId=5   --Exported
                            AND basi.ApplyForId=@ApplyForId
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
                                    INNER JOIN TrnICardRequest req on step.RequestId=req.RequestId
                                    INNER JOIN BasicDetails basi on req.BasicDetailId=basi.BasicDetailId
                                    INNER JOIN MArmedType marmed on basi.ArmedId=marmed.ArmedId
                                    INNER JOIN MRank ranks on ranks.RankId=basi.RankId
                                    INNER JOIN MapUnit unit on basi.UnitId=unit.UnitMapId";
                whereClause = $@"WHERE
                            {unitFilter}
                            AND req.StatusId=1
                            AND step.StepId=6 --I-CARD PRINT
                            AND basi.ApplyForId=@ApplyForId
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
                fromJoinClause = $@"from TrnDispatchCardMapping dcm
                                    INNER JOIN TrnDispatchCard dcard on dcm.DispatchCardId =dcard.DispatchCardId
                                    INNER JOIN TrnICardRequest req on dcm.RequestId=req.RequestId
                                    INNER JOIN TrnStepCounter step on req.RequestId=step.RequestId
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
                            AND req.StatusId=1
                            AND step.StepId=11 --Card Dispatch to Regiment / Officer Record Office
                            AND dcard.Step=1
                            AND dcard.ApplyForId=@ApplyForId
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
                fromJoinClause = $@"from TrnDispatchCardMapping dcm
                                    INNER JOIN TrnDispatchCard dcard on dcm.DispatchCardId =dcard.DispatchCardId
                                    INNER JOIN TrnICardRequest req on dcm.RequestId=req.RequestId
                                    INNER JOIN TrnStepCounter step on req.RequestId=step.RequestId
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
                            AND req.StatusId=1
                            AND step.StepId=12 --Card in Regiment / Officer Record Office
                            AND dcard.Step=1
                            AND dcard.ApplyForId=@ApplyForId
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
                fromJoinClause = $@"from TrnDispatchCardMapping dcm
                                    INNER JOIN TrnDispatchCard dcard on dcm.DispatchCardId =dcard.DispatchCardId
                                    INNER JOIN TrnICardRequest req on dcm.RequestId=req.RequestId
                                    INNER JOIN TrnStepCounter step on req.RequestId=step.RequestId
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
                            AND req.StatusId=1
                            AND step.StepId=13 --Card Dispatch to Unit
                            AND dcard.Step=2
                            AND dcard.ApplyForId={dTO.ApplyForId}
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
                fromJoinClause = $@"from TrnDispatchCardMapping dcm
                                    INNER JOIN TrnDispatchCard dcard on dcm.DispatchCardId =dcard.DispatchCardId
                                    INNER JOIN TrnICardRequest req on dcm.RequestId=req.RequestId
                                    INNER JOIN TrnStepCounter step on req.RequestId=step.RequestId
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
                            AND req.StatusId=1
                            AND step.StepId=14 --Card in Unit
                            AND dcard.Step=2
                            AND dcard.ApplyForId=@ApplyForId
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
                selectFields = $@"req.RequestId,ranks.RankAbbreviation RankName,basi.FName,basi.LName,ServiceNo,marmed.Abbreviation as ArmedAbbreviation,dist.DistributedOn as ActionOn,toRanks.RankAbbreviation as ToRankName,toUp.Name as ToName,toUp.ArmyNo as ToServiceNo,toAspUser.DomainId as ToDID";
                fromJoinClause = $@"from TrnDistributeCards dist
                                    INNER JOIN TrnStepCounter step on dist.RequestId = step.RequestId
                                    INNER JOIN TrnICardRequest req on step.RequestId=req.RequestId
                                    INNER JOIN BasicDetails basi on req.BasicDetailId=basi.BasicDetailId
                                    INNER JOIN MArmedType marmed on basi.ArmedId=marmed.ArmedId
                                    INNER JOIN MRank ranks on ranks.RankId=basi.RankId
                                    INNER JOIN MapUnit unit on basi.UnitId=unit.UnitMapId
                                    INNER JOIN UserProfile toUp on dist.UpdatedbyUserId=toUp.UserId
                                    INNER JOIN MRank toRanks on toUp.RankId=toRanks.RankId
                                    INNER JOIN AspNetUsers toAspUser on dist.Updatedby = toAspUser.Id";
                whereClause = $@"WHERE
                            {unitFilter}
                            AND req.StatusId=2 --Complete
                            AND step.StepId=15 --I- Card Distributed
                            AND basi.ApplyForId=@ApplyForId
                            AND (
	                            @SearchTerm IS NULL OR
                                basi.ServiceNo LIKE '%' + @SearchTerm + '%'
                                )";
            }
                try
                {
                    var sortColumn = allowedSortColumns.ContainsKey(dTO.sortColumn ?? "")
                    ? allowedSortColumns[dTO.sortColumn!]
                    : "ServiceNo";
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
                        draw = 0,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = dTOUserRegnResponses
                    };
                    return responseData;
                }
        }
    }
}