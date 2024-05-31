using Azure.Core;
using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DataAccessLayer
{
    public class TrnFwnDB : GenericRepositoryDL<MTrnFwd>, ITrnFwnDB
    {
        private readonly DapperContext _contextDP;
        protected readonly ApplicationDbContext _context;
        private readonly ILogger<TrnFwnDB> _logger;
        public TrnFwnDB(ApplicationDbContext context, DapperContext contextDP, ILogger<TrnFwnDB> logger) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
        }
        private readonly IConfiguration configuration;

        public async Task<bool> UpdateAllBYRequestId(int RequestId)
        {
            using (var connection = _contextDP.CreateConnection())
            {
                connection.Execute("UPDATE TrnFwds set IsComplete=1 where RequestId=@RequestId", new { RequestId });
                return await Task.FromResult(true);
            }
        }
        public async Task<bool> UpdateFieldBYTrnFwdId(int TrnFwdId)
        {
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    connection.Execute("UPDATE TrnFwds set FwdStatusId=2 where TrnFwdId=@TrnFwdId and FwdStatusId != 3", new { TrnFwdId });
                    return await Task.FromResult(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "TrnFwnDB->UpdateFieldBYTrnFwdId");
                return false;
            }

        }
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
                    foreach (int item in dTO.TrnFwdIds)
                    {
                        MTrnFwd? mTrnFwd = await _context.TrnFwds.FindAsync(item);
                        if(mTrnFwd!=null)
                        {
                            mTrnFwd.IsComplete = true;
                            mTrnFwd.Updatedby = dTO.FromAspNetUsersId;
                            mTrnFwd.UpdatedOn = dTO.UpdatedOn;
                            await _context.SaveChangesAsync();

                            var trnfwd = new MTrnFwd
                            {
                                RequestId = mTrnFwd.RequestId,
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
                                PostingOutId = null,
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
