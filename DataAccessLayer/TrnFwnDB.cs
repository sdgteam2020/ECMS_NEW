using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
            #region old code
            //using (var transaction = _context.Database.BeginTransaction())
            //{
            //    try
            //    {
            //        foreach (int item in dTO.RequestIds)
            //        {
            //            MStepCounter? mStepCounter = await _context.TrnStepCounter.FirstOrDefaultAsync(x => x.RequestId == item);
            //            if (mStepCounter != null)
            //            {
            //                byte StepId = mStepCounter.StepId;
            //                mStepCounter.StepId = (byte)(StepId + 1);
            //                mStepCounter.Updatedby = dTO.FromAspNetUsersId;
            //                mStepCounter.UpdatedOn = dTO.UpdatedOn;
            //                await _context.SaveChangesAsync();

            //                var trnfwd = new MTrnFwd
            //                {
            //                    RequestId = item,
            //                    ToUserId = dTO.ToUserId,
            //                    FromUserId = dTO.FromUserId,
            //                    FromAspNetUsersId = dTO.FromAspNetUsersId,
            //                    ToAspNetUsersId = dTO.ToAspNetUsersId,
            //                    UnitId = dTO.UnitId,
            //                    Remark = dTO.Remark,
            //                    Status = dTO.Status,
            //                    TypeId = dTO.TypeId,
            //                    IsComplete = dTO.IsComplete,
            //                    RemarksIds = dTO.RemarksIds,
            //                    PostingOutId = null,
            //                    IsActive = dTO.IsActive,
            //                    Updatedby = dTO.FromAspNetUsersId,
            //                    UpdatedOn = dTO.UpdatedOn,
            //                };
            //                await _context.TrnFwds.AddAsync(trnfwd);
            //                await _context.SaveChangesAsync();
            //            }
            //            else
            //            {
            //                return false;
            //            }
            //        }
            //        transaction.Commit();
            //        return true;
            //    }
            //    catch (Exception ex)
            //    {
            //        transaction.Rollback();
            //        _logger.LogError(1001, ex, "TrnFwnDB->SaveInternalFwd");
            //        return null;
            //    }
            //}
            #endregion end old code
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
