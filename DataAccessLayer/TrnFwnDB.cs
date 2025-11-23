using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

namespace DataAccessLayer
{
    public class TrnFwnDB : GenericRepositoryDL<MTrnFwd>, ITrnFwnDB
    {
        private readonly DapperContext _contextDP;
        protected new readonly ApplicationDbContext _context;
        private readonly ILogger<TrnFwnDB> _logger;
        public TrnFwnDB(ApplicationDbContext context, DapperContext contextDP, ILogger<TrnFwnDB> logger) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
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
    }

    
}
