using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Healpers;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAccessLayer
{
    public class TrnFwnDB : GenericRepositoryDL<MTrnFwd>, ITrnFwnDB
    {
        private readonly DapperContext _contextDP;
        private readonly DapperContextDb2 _contextDP2;
        protected new readonly ApplicationDbContext _context;
        private readonly ILogger<TrnFwnDB> _logger;
        public TrnFwnDB(ApplicationDbContext context, DapperContext contextDP, DapperContextDb2 contextDP2, ILogger<TrnFwnDB> logger) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _contextDP2 = contextDP2;
            _logger = logger;
        }


        /// <summary>
        /// Updates a specific field in the "TrnFwds" table based on the "TrnFwdId". It checks conditions for FwdStatusId 
        /// and updates it accordingly while performing additional logic if needed.
        /// </summary>
        /// <param name="TrnFwdId">The ID of the record to be updated in the "TrnFwds" table.</param>
        /// <returns>Returns true if the update was successful, otherwise false.</returns>
        public async Task<bool> UpdateFieldBYTrnFwdId(int TrnFwdId)
        {
            try
            {
                MTrnFwd? mTrnFwd = await _context.TrnFwds.FindAsync(TrnFwdId);
                if(mTrnFwd!=null)
                {
                    if(mTrnFwd.FwdStatusId==4)
                    {
                        MTrnFwd? mTrnFwd1 = await _context.TrnFwds.FirstOrDefaultAsync(x => x.RequestId == mTrnFwd.RequestId && x.ToAspNetUsersId == mTrnFwd.FromAspNetUsersId && x.UpdatedOn == mTrnFwd.UpdatedOn);
                        if(mTrnFwd1!=null)
                        {
                            mTrnFwd1.FwdStatusId = 2;
                            await _context.SaveChangesAsync();
                        }
                    }

                }
                using (var connection = _contextDP.CreateConnection())
                {
                    connection.Execute("UPDATE TrnFwds set FwdStatusId=2 where TrnFwdId=@TrnFwdId AND FwdStatusId NOT IN (3,4)", new { TrnFwdId });
                    return await Task.FromResult(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "TrnFwnDB->UpdateFieldBYTrnFwdId");
                return false;
            }

        }



        /// <summary>
        /// Saves internal forward information to the "TrnFwds" table. It involves updating existing records 
        /// and inserting new ones based on the provided DTO.
        /// </summary>
        /// <param name="dTO">The data transfer object containing the internal forward information to be saved.</param>
        /// <returns>Returns true if the save operation was successful, false if an error occurred, or null if an exception was thrown.</returns>
        public async Task<DTOGenericResponse<string>> SaveInternalFwd(DTOSaveInternalFwdRequest dTO, List<DTOCheckRequestIdsBeforeInternalFwdResponse> dTOChecks)
        {
            DTOGenericResponse<string> response = new DTOGenericResponse<string>();
            try
            {
                string[] columnsToIgnore = { "IsValid", "Remarks" };
                byte StepId = 3;
                using (var connection = _contextDP.CreateConnection())
                {

                    foreach (var batchRecords in dTOChecks.Chunk(5000))
                    {

                        DataTable dataTable = DataTableHelper.ToDataTable(batchRecords, columnsToIgnore);
                        var parameters = new DynamicParameters();
                        parameters.Add("@data", dataTable.AsTableValuedParameter("UT_InternalFwd"));
                        parameters.Add("@StepId", StepId, DbType.Byte, ParameterDirection.Input);
                        parameters.Add("@ToUserId", dTO.ToUserId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@FromUserId", dTO.FromUserId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@FromAspNetUsersId", dTO.FromAspNetUsersId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@ToAspNetUsersId", dTO.ToAspNetUsersId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@UnitId", dTO.UnitId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@Remark", dTO.Remark, DbType.String, ParameterDirection.Input, 100);
                        parameters.Add("@FwdStatusId", dTO.FwdStatusId, DbType.Byte, ParameterDirection.Input);
                        parameters.Add("@TypeId", dTO.TypeId, DbType.Byte, ParameterDirection.Input);
                        parameters.Add("@IsComplete", dTO.IsComplete, DbType.Boolean, ParameterDirection.Input);
                        parameters.Add("@RemarksIds", dTO.RemarksIds, DbType.String, ParameterDirection.Input, 100);
                        parameters.Add("@IsActive", dTO.IsActive, DbType.Boolean, ParameterDirection.Input);
                        var batchResponse = await connection.QueryFirstOrDefaultAsync<DTOGenericResponse<string>>("CardInternalFwd", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 180);

                        if (batchResponse == null || !batchResponse.Result)
                        {
                            return new DTOGenericResponse<string>
                            {
                                Result = false,
                                Message = batchResponse?.Message ?? "Batch failed."
                            };
                        }
                    }
                    response.Result = true;
                    response.Message = "All ApplId fwd successfully.";
                    response.Value = "Success";
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "TrnFwnDB->SaveInternalFwd");
                response.Message = "Internal Server Error";
                response.Result = false;
            }
            return response;
        }

        public async Task<DTORequestRejectDetailResponse?> RequestRejectDetail(int RequestId)
        {
            string query = @"declare @StepId tinyint 
                            Select @StepId=step.StepId from TrnStepCounter step where step.RequestId=@RequestId
                            IF @StepId = 1
	                            BEGIN
	                            Select step.ApplyForId,step.StepId,tdm.AspNetUsersId as FromAspNetUsersId,tdm.UserId as FromUserId from TrnStepCounter step
	                            INNER JOIN TrnICardRequest ireq on ireq.RequestId = step.RequestId
	                            INNER JOIN TrnDomainMapping tdm on tdm.Id = ireq.TrnDomainMappingId
	                            where step.RequestId=@RequestId
	                            END
                            ELSE IF @StepId = 2 OR @StepId = 3 OR @StepId = 4
	                            Begin
	                            Select step.ApplyForId,step.StepId,fwd.ToAspNetUsersId as FromAspNetUsersId ,tdm_to.UserId as FromUserId,tdm.AspNetUsersId as ToAspNetUsersId,tdm.UserId as ToUserId from TrnStepCounter step
	                            INNER JOIN TrnICardRequest ireq on ireq.RequestId = step.RequestId
	                            INNER JOIN TrnDomainMapping tdm on tdm.Id = ireq.TrnDomainMappingId
	                            INNER JOIN TrnFwds fwd on fwd.RequestId = ireq.RequestId and fwd.TrnFwdId = (Select MAX(fw.TrnFwdId) from TrnFwds fw where fw.RequestId=ireq.RequestId)
	                            INNER JOIN TrnDomainMapping tdm_to on tdm_to.AspNetUsersId = fwd.ToAspNetUsersId
	                            where step.RequestId=@RequestId
	                            End";
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    DTORequestRejectDetailResponse? dTORequest = (await connection.QueryAsync<DTORequestRejectDetailResponse>(query, new { RequestId })).FirstOrDefault();
                    return dTORequest;
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "TrnFwnDB->RequestRejectDetail");
                return new DTORequestRejectDetailResponse();
            }
        }
        public async Task<DTORequestFwdDetailResponse?> RequestFwdDetail(int RequestId)
        {
            string query = @"declare @ApplyForId tinyint 
                            declare @StepId tinyint 
                            Select @StepId=step.StepId from TrnStepCounter step where step.RequestId=@RequestId
                            IF @StepId = 1
                            BEGIN
                            Select step.ApplyForId,step.StepId,tdm.AspNetUsersId as FromAspNetUsersId,tdm.UserId as FromUserId from TrnStepCounter step
                            INNER JOIN TrnICardRequest ireq on ireq.RequestId = step.RequestId
                            INNER JOIN TrnDomainMapping tdm on tdm.Id = ireq.TrnDomainMappingId
                            where step.RequestId=@RequestId
                            END
                            ELSE IF @StepId = 2
	                            BEGIN
	                            Select @ApplyForId=bs.ApplyForId from BasicDetails bs where bs.BasicDetailId =(Select BasicDetailId from TrnICardRequest where RequestId =@RequestId)
		                            IF @ApplyForId = 1
		                            Begin
		                            Select step.ApplyForId,step.StepId,fwd.ToAspNetUsersId as FromAspNetUsersId ,tdm_to.UserId as FromUserId,tdm.AspNetUsersId as ToAspNetUsersId,tdm.UserId as ToUserId from TrnStepCounter step
		                            INNER JOIN TrnICardRequest ireq on ireq.RequestId = step.RequestId
		                            INNER JOIN MRecordOffice mrec on mrec.RecordOfficeId = ireq.RecordOfficeId
		                            INNER JOIN OROMapping oro on oro.RecordOfficeId = mrec.RecordOfficeId
		                            INNER JOIN TrnDomainMapping tdm on tdm.Id = oro.TDMId
		                            INNER JOIN TrnFwds fwd on fwd.RequestId = ireq.RequestId and fwd.TrnFwdId = (Select MAX(fw.TrnFwdId) from TrnFwds fw where fw.RequestId=ireq.RequestId)
		                            INNER JOIN TrnDomainMapping tdm_to on tdm_to.AspNetUsersId = fwd.ToAspNetUsersId
		                            where step.RequestId=@RequestId
		                            End
		                            ELSE
		                            Begin
		                            Select step.ApplyForId,step.StepId,fwd.ToAspNetUsersId as FromAspNetUsersId ,tdm_to.UserId as FromUserId,tdm.AspNetUsersId as ToAspNetUsersId,tdm.UserId as ToUserId from TrnStepCounter step
		                            INNER JOIN TrnICardRequest ireq on ireq.RequestId = step.RequestId
		                            INNER JOIN MRecordOffice mrec on mrec.RecordOfficeId = ireq.RecordOfficeId
		                            INNER JOIN TrnDomainMapping tdm on tdm.Id = mrec.TDMId
		                            INNER JOIN TrnFwds fwd on fwd.RequestId = ireq.RequestId and fwd.TrnFwdId = (Select MAX(fw.TrnFwdId) from TrnFwds fw where fw.RequestId=ireq.RequestId)
		                            INNER JOIN TrnDomainMapping tdm_to on tdm_to.AspNetUsersId = fwd.ToAspNetUsersId
		                            where step.RequestId=@RequestId
		                            End
	                            END
                            ELSE IF @StepId = 3
                            BEGIN
	                            Select step.ApplyForId,step.StepId,fwd.ToAspNetUsersId as FromAspNetUsersId ,tdm_to.UserId as FromUserId,tdm.AspNetUsersId as ToAspNetUsersId,tdm.UserId as ToUserId from TrnStepCounter step
	                            INNER JOIN TrnICardRequest ireq on ireq.RequestId = step.RequestId
	                            INNER JOIN TrnDomainMapping tdm on tdm.Id = (Select TDMId from AfsacCellMapping where AfsacCellMappingId=1)
	                            INNER JOIN TrnFwds fwd on fwd.RequestId = ireq.RequestId and fwd.TrnFwdId = (Select MAX(fw.TrnFwdId) from TrnFwds fw where fw.RequestId=ireq.RequestId)
	                            INNER JOIN TrnDomainMapping tdm_to on tdm_to.AspNetUsersId = fwd.ToAspNetUsersId
	                            where step.RequestId=@RequestId
                            END
                            ELSE IF @StepId = 7 OR @StepId = 8 OR @StepId = 9
                            BEGIN
                            Select step.ApplyForId,step.StepId,tdm.AspNetUsersId as FromAspNetUsersId,tdm.UserId as FromUserId from TrnStepCounter step
                            INNER JOIN TrnICardRequest ireq on ireq.RequestId = step.RequestId
                            INNER JOIN TrnDomainMapping tdm on tdm.Id = ireq.TrnDomainMappingId
                            where step.RequestId=@RequestId
                            END";
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    DTORequestFwdDetailResponse? dTORequest = (await connection.QueryAsync<DTORequestFwdDetailResponse>(query, new { RequestId })).FirstOrDefault();
                    return dTORequest;
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "TrnFwnDB->RequestFwdDetail");
                return new DTORequestFwdDetailResponse();
            }
        }

        public async Task<bool> ActionOnRequest(DTOActionOnRequest data, byte StepId)
        {
            // Initialize transaction for multiple database operations
            var (db, transaction) = _contextDP.CreateConnectionWithTransaction();
            var (db2, transaction2) = _contextDP2.CreateConnectionWithTransaction();
            try
            {
                string insertSql;
                string query1, query2, query3,query4;

                if (data.Flag == "R")
                {
                    if (StepId == 3)
                    {
                        query2 = @"Update BasicDetails set DateOfIssue=GETDATE() where BasicDetailId=(select BasicDetailId from TrnICardRequest where RequestId=@RequestId)";
                        var parameters2 = new DynamicParameters();
                        parameters2.Add("@RequestId", data.RequestId, DbType.Int32, ParameterDirection.Input);
                        await db.ExecuteAsync(query2, parameters2, transaction: transaction);
                    }

                    query4 = @"UPDATE [dbo].[XmlFilesFwdLog] SET [XmlFiles] ='' ,[Updatedby] = @Updatedby,[UpdatedOn] = @UpdatedOn WHERE [RequestId]= @RequestId";
                    var parameters4 = new DynamicParameters();
                    parameters4.Add("@RequestId", data.RequestId, DbType.Int32, ParameterDirection.Input);
                    parameters4.Add("@Updatedby", data.Updatedby, DbType.Int32, ParameterDirection.Input);
                    parameters4.Add("@UpdatedOn", data.UpdatedOn, DbType.DateTime, ParameterDirection.Input);
                    
                    await db2.ExecuteAsync(query4, parameters4, transaction: transaction2);
                }
                query2 = @"Update BasicDetails set IsLock=@IsLock where BasicDetailId=(select BasicDetailId from TrnICardRequest where RequestId=@RequestId)";
                var parameters5 = new DynamicParameters();
                parameters5.Add("@RequestId", data.RequestId, DbType.Int32, ParameterDirection.Input);
                parameters5.Add("@IsLock", data.IsLock, DbType.Boolean, ParameterDirection.Input);
                await db.ExecuteAsync(query2, parameters5, transaction: transaction);

                query1 = @"Update TrnStepCounter set StepId=@StepId,Updatedby=@Updatedby where RequestId=@RequestId";
                var parameters1 = new DynamicParameters();
                parameters1.Add("@StepId", data.StepId, DbType.Byte, ParameterDirection.Input);
                parameters1.Add("@Updatedby", data.Updatedby, DbType.Int32, ParameterDirection.Input);
                parameters1.Add("@RequestId", data.RequestId, DbType.Int32, ParameterDirection.Input);
                await db.ExecuteAsync(query1, parameters1, transaction: transaction);

                query3 = @"UPDATE TrnFwds set IsComplete=1 where RequestId=@RequestId";
                var parameters3 = new DynamicParameters();
                parameters3.Add("@RequestId", data.RequestId, DbType.Int32, ParameterDirection.Input);
                await db.ExecuteAsync(query3, parameters3, transaction: transaction);


                // Insert new posting record
                insertSql = @$"INSERT INTO TrnFwds(RequestId,ToUserId,FromUserId,FromAspNetUsersId,ToAspNetUsersId,UnitId,Remark,TypeId,IsComplete,IsActive,Updatedby,UpdatedOn,RemarksIds,FwdStatusId,StepId)
                                VALUES (@RequestId,@ToUserId,@FromUserId,@FromAspNetUsersId,@ToAspNetUsersId,@UnitId,@Remark,@TypeId,@IsComplete,@IsActive,@Updatedby,@UpdatedOn,@RemarksIds,@FwdStatusId,@StepId);";
                var parameters = new DynamicParameters();
                parameters.Add("@RequestId", data.RequestId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@ToUserId", data.ToUserId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@FromUserId", data.FromUserId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@FromAspNetUsersId", data.FromAspNetUsersId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@ToAspNetUsersId", data.ToAspNetUsersId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@UnitId", data.UnitId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Remark", data.Remark, DbType.String, ParameterDirection.Input, 100);
                parameters.Add("@TypeId", data.TypeId, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@IsComplete", data.IsComplete, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@IsActive", data.IsActive, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@Updatedby", data.Updatedby, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@UpdatedOn", data.UpdatedOn, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@RemarksIds", data.RemarksIds, DbType.String, ParameterDirection.Input, 100);
                parameters.Add("@FwdStatusId", data.FwdStatusId, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@StepId", data.StepId, DbType.Byte, ParameterDirection.Input);

                // Insert the new TrnFwds record
                await db.ExecuteAsync(insertSql, parameters, transaction: transaction);

                // Commit the transaction if all operations succeed
                transaction.Commit();
                transaction2.Commit();
                return true;
            }
            catch (Exception ex)
            {
                // Rollback the transaction if any operation fails
                transaction.Rollback();
                transaction2.Rollback();
                _logger.LogError(1001, ex, "TrnFwnDB->ActionOnRequest");
                return false;
            }
            finally
            {
                // Dispose of the connection
                db.Dispose();
                db2.Dispose();
            }
        }

        public async Task<DTOCheckUserIdBeforeInternalFwdResponse> CheckUserIdBeforeInternalFwd(int ToAspNetUsersId,int UnitId)
        {
            var response = new DTOCheckUserIdBeforeInternalFwdResponse();
            try
            {
                string query = @"SELECT tdm.UserId,
                                        CASE 
                                            WHEN tdm.UnitId != @UnitId THEN 0
                                            WHEN tdm.UserId = null  THEN 0
                                            ELSE 1
                                        END AS Result,
		                                case
                                            WHEN tdm.UnitId != @UnitId THEN 'The receiver unit does not match the sender unit.'
                                            WHEN tdm.UserId = null THEN 'Profile is not mapped with domain Id!'
		                                    ELSE 'Valid'
		                                END as Message
                                FROM TrnDomainMapping tdm
                                WHERE tdm.AspNetUsersId = @ToAspNetUsersId";

                using (var connection = _contextDP.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@ToAspNetUsersId", ToAspNetUsersId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@UnitId", UnitId, DbType.Int32, ParameterDirection.Input);
                    var result = await connection.QueryFirstOrDefaultAsync<DTOCheckUserIdBeforeInternalFwdResponse>(query, parameters);
                    return result ?? new DTOCheckUserIdBeforeInternalFwdResponse
                    {
                        Result = false,
                        Message = "Receiver Id not found",
                        UserId = 0
                    };
                }
            }
            catch (Exception ee)
            {
                _logger.LogError(1001, ee, "TrnFwnDB->CheckUserIdBeforeInternalFwd");
                response.Result = false;
                response.Message = "Something went wrong";
                response.UserId = 0;
                return response;
            }
        }
        public async Task<List<DTOCheckRequestIdsBeforeInternalFwdResponse>> CheckRequestIdsBeforeInternalFwd(int[] RequestIds, int FromAspNetUsersId)
        {
            var response = new DTOCheckRequestIdsBeforeInternalFwdResponse();
            try
            {
                if (RequestIds == null)
                    return new List<DTOCheckRequestIdsBeforeInternalFwdResponse>();

                var finalResult = new List<DTOCheckRequestIdsBeforeInternalFwdResponse>();

                const string sql = @"
                                    SELECT
                                        ISNULL(fwd.TrnFwdId, 0) AS TrnFwdId,
                                        ISNULL(req.RequestId, 0) AS ApplId,
                                        CAST(
                                            CASE
                                                WHEN req.RequestId IS NOT NULL
                                                        AND step.RequestId IS NOT NULL
                                                        AND fwd.RequestId IS NOT NULL
                                                        AND req.StatusId = 1 --Running
                                                        AND step.StepId = 3 --Pending Appl </br> (Verifier Level )
                                                        AND step.ApplyForId = 2 -- JCO/OR
                                                        AND fwd.ToAspNetUsersId = @FromAspNetUsersId
                                                THEN 1
                                                ELSE 0
                                            END AS bit
                                        ) AS IsValid,
                                        (
                                            CASE
                                                WHEN req.RequestId IS NULL
                                                THEN 'Appl number not exists; '
                                                ELSE ''
                                            END
                                            +
                                            CASE
                                                WHEN req.RequestId IS NOT NULL
                                                        AND req.StatusId <> 1
                                                THEN 'The application is not running; '
                                                ELSE ''
                                            END
                                            +
                                            CASE
                                                WHEN step.RequestId IS NOT NULL
                                                        AND step.ApplyForId <> 2
                                                THEN 'The application is not JCO/OR; '
                                                ELSE ''
                                            END
                                            +
                                            CASE
                                                WHEN step.RequestId IS NOT NULL
                                                        AND step.StepId <> 3
                                                THEN 'The application is currently being processed; '
                                                ELSE ''
                                            END
                                            +
                                            CASE
                                                WHEN req.RequestId IS NOT NULL
                                                        AND fwd.ToAspNetUsersId IS NOT NULL
                                                        AND fwd.ToAspNetUsersId <> @FromAspNetUsersId
                                                THEN 'You are not an authorized user; '
                                                ELSE ''
                                            END
                                        ) AS Remarks
                                    FROM @BatchRecords b
                                    LEFT JOIN TrnICardRequest req ON b.RequestId = req.RequestId
                                    LEFT JOIN TrnStepCounter step ON req.RequestId = step.RequestId
                                    LEFT JOIN TrnFwds fwd on req.RequestId = fwd.RequestId AND fwd.IsComplete=0";

                using (var connection = _contextDP.CreateConnection())
                {
                    foreach (var chunk in RequestIds.Chunk(5000))
                    {
                        var table = new DataTable();
                        table.Columns.Add("RequestId", typeof(int));

                        foreach (var record in chunk)
                        {
                            table.Rows.Add(record);
                        }

                        var parameters = new DynamicParameters();
                        parameters.Add("@FromAspNetUsersId", FromAspNetUsersId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@BatchRecords", table.AsTableValuedParameter("dbo.RequestIdList"));

                        var result = await connection.QueryAsync<DTOCheckRequestIdsBeforeInternalFwdResponse>(sql, parameters);
                        finalResult.AddRange(result);
                    }
                    return finalResult;

                }
            }
            catch (Exception ee)
            {
                _logger.LogError(1001, ee, "TrnFwnDB->CheckUserIdBeforeInternalFwd");
                return new List<DTOCheckRequestIdsBeforeInternalFwdResponse>();
            }
        }
    }
}
