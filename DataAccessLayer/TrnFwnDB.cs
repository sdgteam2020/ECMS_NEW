using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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


        public async Task<bool> AddTrnFwdWithIsCompleteUpdate(MTrnFwd data)
        {
            // Initialize transaction for multiple database operations
            var (db, transaction) = _contextDP.CreateConnectionWithTransaction();
            try
            {
                // Update related TrnICardRequest with the new mapping
                string query1 = "UPDATE TrnFwds set IsComplete=1 where RequestId=@RequestId";
                await db.ExecuteAsync(query1, new { data.RequestId }, transaction: transaction);

                // Insert new posting record
                var insertSql = @$"INSERT INTO TrnFwds(RequestId,ToUserId,FromUserId,FromAspNetUsersId,ToAspNetUsersId,UnitId,Remark,TypeId,IsComplete,IsActive,Updatedby,UpdatedOn,RemarksIds,FwdStatusId,StepId)
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
                return true;
            }
            catch (Exception ex)
            {
                // Rollback the transaction if any operation fails
                transaction.Rollback();
                _logger.LogError(1001, ex, "PostingDB->UpdateForPosting");
                return false;
            }
            finally
            {
                // Dispose of the connection
                db.Dispose();
            }
        }


        /// <summary>
        /// Updates the "IsComplete" status for all records with the specified RequestId in the "TrnFwds" table.
        /// </summary>
        /// <param name="RequestId">The RequestId of the records to be updated.</param>
        /// <returns>Returns true if the update was successful, otherwise false.</returns>
        public async Task<bool> UpdateAllBYRequestId(int RequestId)
        {
            using (var connection = _contextDP.CreateConnection())
            {
                connection.Execute("UPDATE TrnFwds set IsComplete=1 where RequestId=@RequestId", new { RequestId });
                return await Task.FromResult(true);
            }
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
        public async Task<bool?> SaveInternalFwd(DTOSaveInternalFwdRequest dTO)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    foreach (int item in dTO.RequestIds)
                    {
                        //MTrnFwd? mTrnFwd = await _context.TrnFwds.FindAsync(item);
                        MTrnFwd? mTrnFwd = await _context.TrnFwds.Where(x=>x.RequestId == item && x.IsComplete==false).FirstOrDefaultAsync();
                        if (mTrnFwd!=null)
                        {
                            mTrnFwd.IsComplete = true;
                            mTrnFwd.Updatedby = dTO.FromAspNetUsersId;
                            mTrnFwd.UpdatedOn = dTO.UpdatedOn;
                            await _context.SaveChangesAsync();

                            var trnfwd = new MTrnFwd
                            {
                                RequestId = mTrnFwd.RequestId,
                                StepId = mTrnFwd.StepId,
                                ToUserId = dTO.ToUserId,
                                FromUserId = dTO.FromUserId,
                                FromAspNetUsersId = dTO.FromAspNetUsersId,
                                ToAspNetUsersId = dTO.ToAspNetUsersId,
                                UnitId = dTO.UnitId,
                                Remark = dTO.Remark,
                                FwdStatusId = dTO.FwdStatusId,
                                TypeId = dTO.TypeId,
                                IsComplete = dTO.IsComplete,
                                RemarksIds = dTO.RemarksIds,
                                //PostingOutId = null,
                                IsActive = dTO.IsActive,
                                Updatedby = dTO.FromAspNetUsersId,
                                UpdatedOn = dTO.UpdatedOn,
                            };
                            await _context.TrnFwds.AddAsync(trnfwd);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            return false;
                        }
                    }
                    transaction.Commit();
                    return await Task.FromResult(true); ;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError(1001, ex, "TrnFwnDB->SaveInternalFwd");
                    return null;
                }
            }
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
    }
}
