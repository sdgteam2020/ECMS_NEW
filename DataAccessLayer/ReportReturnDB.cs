using Azure.Core;
using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        public Task<List<DTOReportReturnCount>> GetMstepCountApprovedRejectJco(DTOMHierarchyRequest Data, int ApplyForId)
        {
            throw new NotImplementedException();
        }



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

        public async Task<List<DTOReportReturnCount>> GetRecordOffOffersCount(DTOMHierarchyRequest Data)
        {
            //string query = " select count(req.RequestId) Total ,recf.RecordOfficeId,recf.Name from MRecordOffice recf" +
            //               " left join TrnDomainMapping map on recf.TDMId=map.Id" +
            //               " left join TrnFwds fwd on map.AspNetUsersId=fwd.FromAspNetUsersId and fwd.FwdStatusId=1" +
            //               " left join TrnICardRequest req on fwd.RequestId=req.RequestId and req.StatusId=1  " +
            //               " left join MRecordOffice mrec on map.Id=mrec.TDMId and mrec.ArmedId=56 " +
            //               " left join BasicDetails basi on req.BasicDetailId=basi.BasicDetailId " +
            //               " left join MapUnit unit on basi.UnitId=unit.UnitMapId" +
            //               " and unit.ComdId=ISNULL(@ComdId,unit.ComdId) " +
            //               " and unit.CorpsId=ISNULL(@CorpsId,unit.CorpsId)" +
            //               " and unit.DivId=ISNULL(@DivId,unit.DivId)" +
            //               " and unit.BdeId=ISNULL(@BdeId,unit.BdeId)" +
            //               " and unit.BdeId=ISNULL(@FmnBranchID,unit.FmnBranchID)" +
            //               " and unit.BdeId=ISNULL(@PsoId,unit.PsoId)" +
            //               " and unit.BdeId=ISNULL(@SubDteId,unit.SubDteId)" +
            //               " and unit.UnitMapId=ISNULL(@UnitMapId,unit.UnitMapId)" +
            //               " where recf.ArmedId=56 group by recf.RecordOfficeId,recf.Name";

            string query = " select COUNT(req.RequestId) Total, fwdsts.Name,fwdsts.FwdStatusId,mrec.RecordOfficeId, 3 GroupId from MTrnFwdStatus fwdsts" +
                           " inner join TrnFwds fwd on fwdsts.FwdStatusId=fwd.FwdStatusId  " +
                           " inner join TrnICardRequest req on fwd.RequestId=req.RequestId and req.StatusId=1 " +
                           " inner join TrnStepCounter step on step.ApplyForId=1 and req.RequestId=step.RequestId" +
                           " inner join TrnDomainMapping map on fwd.ToAspNetUsersId=map.AspNetUsersId  " +
                           " inner join OROMapping mrec on map.Id=mrec.TDMId " +
                           " inner join  BasicDetails basi on req.BasicDetailId=basi.BasicDetailId " +
                           " left join MapUnit unit on basi.UnitId=unit.UnitMapId " +
                          " where unit.ComdId=ISNULL(@ComdId,unit.ComdId) " +
                           " and unit.CorpsId=ISNULL(@CorpsId,unit.CorpsId)" +
                           " and unit.DivId=ISNULL(@DivId,unit.DivId)" +
                           " and unit.BdeId=ISNULL(@BdeId,unit.BdeId)" +
                           //" and unit.FmnBranchID=ISNULL(@FmnBranchID,unit.FmnBranchID)" +
                           //" and unit.PsoId=ISNULL(@PsoId,unit.PsoId)" +
                           //" and unit.SubDteId=ISNULL(@SubDteId,unit.SubDteId)" +
                           " and unit.UnitMapId=ISNULL(@UnitMapId,unit.UnitMapId)" +
                           //" where fwd.IsComplete=0" +
                           " group by fwdsts.Name,fwdsts.FwdStatusId,mrec.RecordOfficeId";
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

        public async Task<List<DTOReportReturnCount>> GetRecordJcoCount(DTOMHierarchyRequest Data, int IsComplete, short ArmedIdForORO)
        {
            string query = " select count(req.RequestId) Total ,recf.RecordOfficeId,recf.Name,step.StepId from MRecordOffice recf" +
                           " left join TrnDomainMapping map on recf.TDMId=map.Id" +
                           " left join TrnFwds fwd on map.AspNetUsersId=fwd.ToAspNetUsersId and fwd.IsComplete=@IsComplete and fwd.StepId=3" +
                           " left join TrnICardRequest req on fwd.RequestId=req.RequestId and req.StatusId=1  " +
                           " left join TrnStepCounter step on req.RequestId=step.RequestId " +
                           " left join MRecordOffice mrec on map.Id=mrec.TDMId and mrec.ArmedId!=@ArmedIdForORO " +
                           " left join BasicDetails basi on req.BasicDetailId=basi.BasicDetailId " +
                           " left join MapUnit unit on basi.UnitId=unit.UnitMapId" +
                         " where unit.ComdId=ISNULL(@ComdId,unit.ComdId) " +
                           " and unit.CorpsId=ISNULL(@CorpsId,unit.CorpsId)" +
                           " and unit.DivId=ISNULL(@DivId,unit.DivId)" +
                           " and unit.BdeId=ISNULL(@BdeId,unit.BdeId)" +
                           //" and unit.FmnBranchID=ISNULL(@FmnBranchID,unit.FmnBranchID)" +
                           //" and unit.PsoId=ISNULL(@PsoId,unit.PsoId)" +
                           //" and unit.SubDteId=ISNULL(@SubDteId,unit.SubDteId)" +
                           " and unit.UnitMapId=ISNULL(@UnitMapId,unit.UnitMapId)" +
                           " and recf.ArmedId!=@ArmedIdForORO   group by recf.RecordOfficeId,recf.Name,step.StepId";
            try 
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTOReportReturnCount>(query, new { IsComplete, Data.ComdId, Data.CorpsId, Data.DivId, Data.BdeId, Data.FmnBranchID, Data.PsoId, Data.SubDteId, Data.UnitMapId, ArmedIdForORO });
                    return ret.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "ReportReturnDB->GetReportForm11");
                return new List<DTOReportReturnCount>();
            }
        }

        public async Task<DTODataTablesResponse<DTOReportReturnListResponse>> GetRecordHistory(DTORecordHistory dTORecord)
        {
            // Map allowed sort columns to DB fields
            var allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["ServiceNo"] = "ServiceNo",
                ["TrackingId"] = "TrackingId",
                ["UpdatedOn"] = "fwd.UpdatedOn",
                ["StatusName"] = "fwdsts.Name",
            };

            var sortColumn = allowedSortColumns.ContainsKey(dTORecord.sortColumn ?? "")
                ? allowedSortColumns[dTORecord.sortColumn!]
                : "ServiceNo";

            var sortOrder = dTORecord.sortDirection;

            string query = "";
            if (dTORecord.StepId != 99)
            {
                if(dTORecord.StepId ==100)
                {
                    query = " req.RequestId,Mstep.StepId,basi.FName,basi.LName,ServiceNo,DOB,ranks.RankAbbreviation RankName,TrackingId, " +
                            " aspusersto.DomainId DomainIdTo,userto.ArmyNo ArmyNoTo ,userto.Name NameTo,ranksto.RankAbbreviation RankTo, " +
                            " aspusersfrom.DomainId DomainIdFrom,userfrom.ArmyNo ArmyNoFrom ,userfrom.Name NameFrom,ranksfrom.RankAbbreviation RankFrom,fwdsts.Name Status" +
                            " ,fwd.UpdatedOn,fwdsts.Name StatusName from MStepCounterStep Mstep   " +
                            " inner join TrnStepCounter step on Mstep.StepId=step.StepId  " +
                            " inner join TrnICardRequest req on step.RequestId=req.RequestId and req.StatusId=1  " +
                            " inner join  BasicDetails basi on req.BasicDetailId=basi.BasicDetailId  " +
                            " inner join TrnFwds fwd on req.RequestId=fwd.RequestId " +
                            " inner join MArmedType marmed on basi.ArmedId=marmed.ArmedId  " +
                            " inner join MRecordOffice mrec on marmed.ArmedId=mrec.ArmedId " +
                            " left join UserProfile userto on fwd.ToUserId=userto.UserId  " +
                            " LEFT join TrnDomainMapping mapto on userto.UserId=mapto.UserId  " +
                            " LEFT join AspNetUsers aspusersto on mapto.AspNetUsersId=aspusersto.Id " +
                            " left join MRank ranksto on ranksto.RankId=userto.RankId " +
                            " left join MTrnFwdStatus fwdsts on fwd.FwdStatusId=fwdsts.FwdStatusId " +
                            " left join UserProfile userfrom on fwd.FromUserId=userfrom.UserId  " +
                            " LEFT join TrnDomainMapping mapfrom on userfrom.UserId=mapfrom.UserId  " +
                            " LEFT join AspNetUsers aspusersfrom on mapfrom.AspNetUsersId=aspusersfrom.Id " +
                            " left join MRank ranksfrom on ranksfrom.RankId=userfrom.RankId " +

                            " left join MRank ranks on ranks.RankId=basi.RankId " +
                            " left join MapUnit unit on basi.UnitId=unit.UnitMapId  " +
                           " where unit.ComdId=ISNULL(@ComdId,unit.ComdId) " +
                               " and unit.CorpsId=ISNULL(@CorpsId,unit.CorpsId)" +
                               " and unit.DivId=ISNULL(@DivId,unit.DivId)" +
                               " and unit.BdeId=ISNULL(@BdeId,unit.BdeId)" +
                               //" and unit.FmnBranchID=ISNULL(@FmnBranchID,unit.FmnBranchID)" +
                               //" and unit.PsoId=ISNULL(@PsoId,unit.PsoId)" +
                               //" and unit.SubDteId=ISNULL(@SubDteId,unit.SubDteId)" +
                               " and unit.UnitMapId=ISNULL(@UnitMapId,unit.UnitMapId)" +
                            " and step.ApplyForId=2 and fwd.IsComplete=0 and fwd.StepId=3 and mrec.RecordOfficeId=@ApplyForId and ServiceNo like '%' + @SearchTerm + '%' ";
                }
                else if (dTORecord.IsApproveId == 1)
                {
                    query = " req.RequestId,Mstep.StepId,basi.FName,basi.LName,ServiceNo,DOB,ranks.RankAbbreviation RankName,TrackingId," +
                            " aspusersto.DomainId DomainIdTo,userto.ArmyNo ArmyNoTo ,userto.Name NameTo,ranksto.RankAbbreviation RankTo," +
                            " aspusersfrom.DomainId DomainIdFrom,userfrom.ArmyNo ArmyNoFrom ,userfrom.Name NameFrom,ranksfrom.RankAbbreviation RankFrom" +
                            " ,fwd.UpdatedOn,fwdsts.Name StatusName from MStepCounterStep Mstep  " +
                            " left join TrnStepCounter step on Mstep.StepId=step.StepId " +
                            " left join TrnICardRequest req on step.RequestId=req.RequestId and req.StatusId=1 " +
                            " left join TrnFwds fwd on req.RequestId=fwd.RequestId " +
                            " left join UserProfile userto on fwd.ToUserId=userto.UserId" +
                            " LEFT join TrnDomainMapping mapto on userto.UserId=mapto.UserId" +
                            " LEFT join AspNetUsers aspusersto on mapto.AspNetUsersId=aspusersto.Id" +
                            " left join MRank ranksto on ranksto.RankId=userto.RankId" +
                            " left join MTrnFwdStatus fwdsts on fwd.FwdStatusId=fwdsts.FwdStatusId" +
                            " left join UserProfile userfrom on fwd.FromUserId=userfrom.UserId" +
                            " LEFT join TrnDomainMapping mapfrom on userfrom.UserId=mapfrom.UserId" +
                            " LEFT join AspNetUsers aspusersfrom on mapfrom.AspNetUsersId=aspusersfrom.Id" +
                            " left join MRank ranksfrom on ranksfrom.RankId=userfrom.RankId" +
                            " left join  BasicDetails basi on req.BasicDetailId=basi.BasicDetailId " +
                            " left join MRank ranks on ranks.RankId=basi.RankId" +
                            " left join MapUnit unit on basi.UnitId=unit.UnitMapId " +
                            " where unit.ComdId=ISNULL(@ComdId,unit.ComdId) " +
                           " and unit.CorpsId=ISNULL(@CorpsId,unit.CorpsId)" +
                           " and unit.DivId=ISNULL(@DivId,unit.DivId)" +
                           " and unit.BdeId=ISNULL(@BdeId,unit.BdeId)" +
                           //" and unit.FmnBranchID=ISNULL(@FmnBranchID,unit.FmnBranchID)" +
                           //" and unit.PsoId=ISNULL(@PsoId,unit.PsoId)" +
                           //" and unit.SubDteId=ISNULL(@SubDteId,unit.SubDteId)" +
                           " and unit.UnitMapId=ISNULL(@UnitMapId,unit.UnitMapId)" +
                            " and step.ApplyForId=@ApplyForId and fwd.StepId=@StepId and ServiceNo like '%' + @SearchTerm + '%' ";
                        
                     }
                else
                {
                    if(dTORecord.StepId ==1)
                    {
                        query = " req.RequestId,Mstep.StepId,basi.FName,basi.LName,ServiceNo,DOB,ranks.RankAbbreviation RankName,TrackingId," +
                      " aspusersto.DomainId DomainIdTo,userto.ArmyNo ArmyNoTo ,userto.Name NameTo,ranksto.RankAbbreviation RankTo," +
                      " aspusersfrom.DomainId DomainIdFrom,userfrom.ArmyNo ArmyNoFrom ,userfrom.Name NameFrom,ranksfrom.RankAbbreviation RankFrom" +
                      " ,fwd.UpdatedOn,fwdsts.Name StatusName from MStepCounterStep Mstep  " +
                      " left join TrnStepCounter step on Mstep.StepId=step.StepId and Mstep.StepId=@StepId" +
                      " left join TrnICardRequest req on step.RequestId=req.RequestId and req.StatusId=1 " +
                      " left join TrnFwds fwd on req.RequestId=fwd.RequestId and fwd.IsComplete=0 " +
                      " left join UserProfile userto on fwd.ToUserId=userto.UserId" +
                      "  LEFT join TrnDomainMapping mapto on userto.UserId=mapto.UserId" +
                      "  LEFT join AspNetUsers aspusersto on mapto.AspNetUsersId=aspusersto.Id" +
                      " left join MRank ranksto on ranksto.RankId=userto.RankId" +
                      " left join MTrnFwdStatus fwdsts on fwd.FwdStatusId=fwdsts.FwdStatusId" +
                      " left join UserProfile userfrom on fwd.FromUserId=userfrom.UserId" +
                      "  LEFT join TrnDomainMapping mapfrom on userfrom.UserId=mapfrom.UserId" +
                      "  LEFT join AspNetUsers aspusersfrom on mapfrom.AspNetUsersId=aspusersfrom.Id" +
                      " left join MRank ranksfrom on ranksfrom.RankId=userfrom.RankId" +
                      " left join  BasicDetails basi on req.BasicDetailId=basi.BasicDetailId " +
                      " left join MRank ranks on ranks.RankId=basi.RankId" +
                      " left join MapUnit unit on basi.UnitId=unit.UnitMapId " +
                      " where unit.ComdId=ISNULL(@ComdId,unit.ComdId) " +
                           " and unit.CorpsId=ISNULL(@CorpsId,unit.CorpsId)" +
                           " and unit.DivId=ISNULL(@DivId,unit.DivId)" +
                           " and unit.BdeId=ISNULL(@BdeId,unit.BdeId)" +
                           //" and unit.FmnBranchID=ISNULL(@FmnBranchID,unit.FmnBranchID)" +
                           //" and unit.PsoId=ISNULL(@PsoId,unit.PsoId)" +
                           //" and unit.SubDteId=ISNULL(@SubDteId,unit.SubDteId)" +
                           " and unit.UnitMapId=ISNULL(@UnitMapId,unit.UnitMapId)" +
                      " and step.ApplyForId=@ApplyForId and ServiceNo like '%' + @SearchTerm + '%' ";
                    }
                    else
                    {
                        //Appl Status at ADC
                        query = " req.RequestId,Mstep.StepId,basi.FName,basi.LName,ServiceNo,DOB,ranks.RankAbbreviation RankName,TrackingId," +
                      " aspusersto.DomainId DomainIdTo,userto.ArmyNo ArmyNoTo ,userto.Name NameTo,ranksto.RankAbbreviation RankTo," +
                      " aspusersfrom.DomainId DomainIdFrom,userfrom.ArmyNo ArmyNoFrom ,userfrom.Name NameFrom,ranksfrom.RankAbbreviation RankFrom" +
                      " ,fwd.UpdatedOn,fwdsts.Name StatusName from MStepCounterStep Mstep  " +
                      " left join TrnStepCounter step on Mstep.StepId=step.StepId " +
                      " left join TrnICardRequest req on step.RequestId=req.RequestId and req.StatusId=1 " +
                      " left join TrnFwds fwd on req.RequestId=fwd.RequestId and fwd.IsComplete=0 " +
                      " left join UserProfile userto on fwd.ToUserId=userto.UserId" +
                      "  LEFT join TrnDomainMapping mapto on userto.UserId=mapto.UserId" +
                      "  LEFT join AspNetUsers aspusersto on mapto.AspNetUsersId=aspusersto.Id" +
                      " left join MRank ranksto on ranksto.RankId=userto.RankId" +
                      " left join MTrnFwdStatus fwdsts on fwd.FwdStatusId=fwdsts.FwdStatusId" +
                      " left join UserProfile userfrom on fwd.FromUserId=userfrom.UserId" +
                      "  LEFT join TrnDomainMapping mapfrom on userfrom.UserId=mapfrom.UserId" +
                      "  LEFT join AspNetUsers aspusersfrom on mapfrom.AspNetUsersId=aspusersfrom.Id" +
                      " left join MRank ranksfrom on ranksfrom.RankId=userfrom.RankId" +
                      " left join  BasicDetails basi on req.BasicDetailId=basi.BasicDetailId " +
                      " left join MRank ranks on ranks.RankId=basi.RankId" +
                      " left join MapUnit unit on basi.UnitId=unit.UnitMapId " +
                      " where unit.ComdId=ISNULL(@ComdId,unit.ComdId) " +
                           " and unit.CorpsId=ISNULL(@CorpsId,unit.CorpsId)" +
                           " and unit.DivId=ISNULL(@DivId,unit.DivId)" +
                           " and unit.BdeId=ISNULL(@BdeId,unit.BdeId)" +
                           //" and unit.FmnBranchID=ISNULL(@FmnBranchID,unit.FmnBranchID)" +
                           //" and unit.PsoId=ISNULL(@PsoId,unit.PsoId)" +
                           //" and unit.SubDteId=ISNULL(@SubDteId,unit.SubDteId)" +
                           " and unit.UnitMapId=ISNULL(@UnitMapId,unit.UnitMapId)" +
                      " and step.ApplyForId=@ApplyForId and fwd.StepId=@StepId and ServiceNo like '%' + @SearchTerm + '%' ";
                    }
                      
                }
            }
            else
            {
                query = " req.RequestId,Mstep.StepId,basi.FName,basi.LName,ServiceNo,DOB,ranks.RankAbbreviation RankName,TrackingId, " +
                        " aspusersto.DomainId DomainIdTo,userto.ArmyNo ArmyNoTo ,userto.Name NameTo,ranksto.RankAbbreviation RankTo, " +
                        " aspusersfrom.DomainId DomainIdFrom,userfrom.ArmyNo ArmyNoFrom ,userfrom.Name NameFrom,ranksfrom.RankAbbreviation RankFrom,fwdsts.Name Status" +
                        " ,fwd.UpdatedOn,fwdsts.Name StatusName from MStepCounterStep Mstep   " +
                        " inner join TrnStepCounter step on Mstep.StepId=step.StepId  " +
                        " inner join TrnICardRequest req on step.RequestId=req.RequestId and req.StatusId=1  " +
                        " inner join TrnFwds fwd on req.RequestId=fwd.RequestId " +
                        " inner join TrnDomainMapping map on fwd.ToAspNetUsersId=map.AspNetUsersId  " +
                        " inner join OROMapping mrec on map.Id=mrec.TDMId " +
                        " left join UserProfile userto on fwd.ToUserId=userto.UserId  " +
                        " LEFT join TrnDomainMapping mapto on userto.UserId=mapto.UserId  " +
                        " LEFT join AspNetUsers aspusersto on mapto.AspNetUsersId=aspusersto.Id " +
                        " left join MRank ranksto on ranksto.RankId=userto.RankId " +
                        " left join MTrnFwdStatus fwdsts on fwd.FwdStatusId=fwdsts.FwdStatusId " +
                        " left join UserProfile userfrom on fwd.FromUserId=userfrom.UserId  " +
                        " LEFT join TrnDomainMapping mapfrom on userfrom.UserId=mapfrom.UserId  " +
                        " LEFT join AspNetUsers aspusersfrom on mapfrom.AspNetUsersId=aspusersfrom.Id " +
                        " left join MRank ranksfrom on ranksfrom.RankId=userfrom.RankId " +
                        " left join  BasicDetails basi on req.BasicDetailId=basi.BasicDetailId  " +
                        " left join MRank ranks on ranks.RankId=basi.RankId " +
                        " left join MapUnit unit on basi.UnitId=unit.UnitMapId  " +
                       " where unit.ComdId=ISNULL(@ComdId,unit.ComdId) " +
                           " and unit.CorpsId=ISNULL(@CorpsId,unit.CorpsId)" +
                           " and unit.DivId=ISNULL(@DivId,unit.DivId)" +
                           " and unit.BdeId=ISNULL(@BdeId,unit.BdeId)" +
                           //" and unit.FmnBranchID=ISNULL(@FmnBranchID,unit.FmnBranchID)" +
                           //" and unit.PsoId=ISNULL(@PsoId,unit.PsoId)" +
                           //" and unit.SubDteId=ISNULL(@SubDteId,unit.SubDteId)" +
                           " and unit.UnitMapId=ISNULL(@UnitMapId,unit.UnitMapId)" +
                        " and step.ApplyForId=1 and fwd.StepId=3 and mrec.RecordOfficeId=@ApplyForId and ServiceNo like '%' + @SearchTerm + '%' ";

            }
            try
            {
                var multiQuery = query = $@"
                            WITH RecordCTE AS (
                                select ROW_NUMBER() OVER (ORDER BY {sortColumn} {sortOrder}) AS RowNum, {query}
                            )
                            SELECT * FROM RecordCTE
                            WHERE RowNum BETWEEN @Offset AND @Limit;
                        ";

                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryMultipleAsync(query, new { dTORecord.Data.ComdId, dTORecord.Data.CorpsId, dTORecord.Data.DivId, dTORecord.Data.BdeId, dTORecord.Data.FmnBranchID, dTORecord.Data.PsoId, dTORecord.Data.SubDteId, dTORecord.Data.UnitMapId, dTORecord.ApplyForId, dTORecord.StepId, Offset = dTORecord.Start, Limit = dTORecord.Length, SearchTerm = string.IsNullOrWhiteSpace(dTORecord.searchValue) ? "" : dTORecord.searchValue });
                    var records = (await ret.ReadAsync<DTOReportReturnListResponse>()).ToList();
                    var responseData = new DTODataTablesResponse<DTOReportReturnListResponse>
                    {
                        draw = dTORecord.Draw,
                        recordsTotal = 0, // Total records without filtering
                        recordsFiltered = records.Count(), // Total records after filtering
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
        public async Task<DTODataTablesResponse<DTOReportResponse>> GetReportData(DTODataTablesRequestForReport dTO)
        {
            string query = "";
            // Map allowed sort columns to DB fields
            Dictionary<string, string> allowedSortColumns = new Dictionary<string, string>();

            var sortOrder = dTO.sortDirection;
            if (dTO.Choice == "Requisition")
            {
                allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["ServiceNo"] = "ServiceNo",
                    ["RequestId"] = "req.RequestId",
                    ["TrackingId"] = "TrackingId",
                    ["StepId"] = "Mstep.StepId",
                    ["ArmedAbbreviation"] = "marmed.Abbreviation",
                    ["ApplyFor"] = "mappl.Name"
                };
                query = @"req.RequestId,Mstep.StepId,basi.FName,basi.LName,basi.NameAsPerRecord,ServiceNo,ranks.RankAbbreviation RankName,TrackingId,marmed.Abbreviation as ArmedAbbreviation,regi.Abbreviation RegimentalName,mappl.Name as ApplyFor,
                            CASE
                            WHEN Mstep.StepId=1 THEN
                            'Drafted/Saved </br> Appl'
                            WHEN Mstep.StepId=2 THEN
                            'Pending Appl </br> (Approver Level)'
                            WHEN Mstep.StepId=3 THEN
                            'Pending Appl </br> (Verifier Level)'
                            WHEN Mstep.StepId=4 THEN
                            'Appl  Status </br> at ADC'
                            WHEN Mstep.StepId=5 THEN
                            'Exported'
                            WHEN Mstep.StepId=6 THEN
                            'I-CARD PRINT'
                            WHEN Mstep.StepId=7 THEN
                            'Appl Rejected  </br> (Approver Level)'
                            WHEN Mstep.StepId=8 THEN
                            'Appl Rejected </br>  (Verifier Level)'
                            WHEN Mstep.StepId=9 THEN
                            'Appl Rejected </br> (4th LEVEL)'
                            END AS Status
                            from TrnStepCounter step
                            INNER JOIN MApplyFor mappl on mappl.ApplyForId=step.ApplyForId
                            INNER JOIN MStepCounterStep Mstep on Mstep.StepId=step.StepId
                            INNER JOIN TrnICardRequest req on step.RequestId=req.RequestId and req.StatusId=1
                            INNER JOIN  BasicDetails basi on req.BasicDetailId=basi.BasicDetailId
                            INNER JOIN MArmedType marmed on basi.ArmedId=marmed.ArmedId
                            INNER JOIN MRank ranks on ranks.RankId=basi.RankId
                            INNER JOIN MapUnit unit on basi.UnitId=unit.UnitMapId
                            left join MRegimental regi on regi.RegId=basi.RegimentalId
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
                            left join MRegimental regi on regi.RegId=bas.RegimentalId
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
                            left join MRegimental regi on regi.RegId=bas.RegimentalId
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
                            AND ServiceNo LIKE '%' + @SearchTerm + '%'";
            }
            else if (dTO.Choice == "MonthlyProcessed")
            {
                allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["ServiceNo"] = "ServiceNo",
                    ["RequestId"] = "req.RequestId",
                    ["TrackingId"] = "TrackingId",
                    ["StepId"] = "Mstep.StepId",
                    ["ArmedAbbreviation"] = "marmed.Abbreviation",
                    ["ApplyFor"] = "mappl.Name"
                };
                query = @"req.RequestId,Mstep.StepId,basi.FName,basi.LName,basi.NameAsPerRecord,ServiceNo,ranks.RankAbbreviation RankName,TrackingId,marmed.Abbreviation as ArmedAbbreviation,regi.Abbreviation RegimentalName,mappl.Name as ApplyFor,basi.UpdatedOn,
                            CASE
                            WHEN Mstep.StepId=1 THEN
                            'Drafted/Saved </br> Appl'
                            WHEN Mstep.StepId=2 THEN
                            'Pending Appl </br> (Approver Level)'
                            WHEN Mstep.StepId=3 THEN
                            'Pending Appl </br> (Verifier Level)'
                            WHEN Mstep.StepId=4 THEN
                            'Appl  Status </br> at ADC'
                            WHEN Mstep.StepId=5 THEN
                            'Exported'
                            WHEN Mstep.StepId=6 THEN
                            'I-CARD PRINT'
                            WHEN Mstep.StepId=7 THEN
                            'Appl Rejected  </br> (Approver Level)'
                            WHEN Mstep.StepId=8 THEN
                            'Appl Rejected </br>  (Verifier Level)'
                            WHEN Mstep.StepId=9 THEN
                            'Appl Rejected </br> (4th LEVEL)'
                            END AS Status
                            from TrnStepCounter step
                            INNER JOIN MApplyFor mappl on mappl.ApplyForId=step.ApplyForId
                            INNER JOIN MStepCounterStep Mstep on Mstep.StepId=step.StepId
                            INNER JOIN TrnICardRequest req on step.RequestId=req.RequestId and req.StatusId=1
                            INNER JOIN  BasicDetails basi on req.BasicDetailId=basi.BasicDetailId
                            INNER JOIN MArmedType marmed on basi.ArmedId=marmed.ArmedId
                            INNER JOIN MRank ranks on ranks.RankId=basi.RankId
                            INNER JOIN MapUnit unit on basi.UnitId=unit.UnitMapId
                            left join MRegimental regi on regi.RegId=basi.RegimentalId
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
                            select ROW_NUMBER() OVER (ORDER BY {sortColumn} {sortOrder}) AS RowNum, {query}
                        )
                        SELECT * FROM RecordCTE
                        WHERE RowNum BETWEEN @Offset AND @Limit;
                    ";

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
                    parameters.Add("@Offset", dTO.Start, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Limit", dTO.Length, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@SearchTerm", dTO.searchValue, DbType.String, ParameterDirection.Input);

                    var ret = await connection.QueryMultipleAsync(query, parameters);
                    var records = (await ret.ReadAsync<DTOReportResponse>()).ToList();
                    var responseData = new DTODataTablesResponse<DTOReportResponse>
                    {
                        draw = dTO.Draw,
                        recordsTotal = 0, // Total records without filtering
                        recordsFiltered = records.Count(), // Total records after filtering
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
        public async Task<DTOReportDashboardCountResponse> GetReportDashboardCount(DTODataTablesRequestForReport dTO, bool Claim)
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
                            AND (
	                            @Claim = 1
	                            OR (@Claim = 0 AND unit.UnitMapId = ISNULL(@UnitMapId, unit.UnitMapId))
                            )
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
                            AND (
	                            @Claim = 1
	                            OR (@Claim = 0 AND unit.UnitMapId = ISNULL(@UnitMapId, unit.UnitMapId))
                            )

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
                            AND (
	                            @Claim = 1
	                            OR (@Claim = 0 AND unit.UnitMapId = ISNULL(@UnitMapId, unit.UnitMapId))
                            )
                            AND req.StatusId=1
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
                            AND (
	                            @Claim = 1
	                            OR (@Claim = 0 AND unit.UnitMapId = ISNULL(@UnitMapId, unit.UnitMapId))
                            )

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
                    parameters.Add("@Claim", Claim, DbType.Boolean, ParameterDirection.Input);

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
        public async Task<List<DTOReportReturnListResponse>> GetReportForm11(DTOMHierarchyRequest Data)
        {
            string query = " select " +
                           "   req.RequestId, " +
                           "   basi.FName,basi.LName, " +
                           "   ServiceNo, " +
                           "   DOB, " +
                           "   ranks.RankAbbreviation RankName, " +
                           "   TrackingId" +
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
    }
}





