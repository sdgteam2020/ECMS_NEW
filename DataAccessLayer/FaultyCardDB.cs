using Azure.Core;
using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAccessLayer
{
    public class FaultyCardDB : GenericRepositoryDL<TrnFaultyCard>, IFaultyCardDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly IDataProtector protector;
        private readonly ILogger<FaultyCardDB> _logger;
        public FaultyCardDB(ApplicationDbContext context, DapperContext contextDP, IDataProtectionProvider dataProtectionProvider, ILogger<FaultyCardDB> logger, DataProtectionPurposeStrings dataProtectionPurposeStrings) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
            // Pass the purpose string as a parameter
            this.protector = dataProtectionProvider.CreateProtector(
                dataProtectionPurposeStrings.AFSACIdRouteValue);
        }
        public async Task<bool> FindRequestId(int RequestId)
        {
            try
            {
                return await _context.TrnFaultyCard
                                .AnyAsync(f => f.RequestId == RequestId &&
                                                _context.TrnICardRequest.Any(req => req.RequestId == RequestId && req.FlagForFaulty));
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "FaultyCardDB->FindRequestId");
                return false;
            }
        }
        public async Task<List<DTOFaultyCardListResponse>?> GetAllFaulty(bool Claim,int MapUnitId)
        {
            try
            {
                string query = "";
                if (Claim)
                {
                    query = @"SELECT appl.Name ApplyFor,mcat.Name FaultyStage,req.RequestId,faulty.TrnFaultyCardId,bas.ServiceNo,ranks.RankAbbreviation RankName,bas.FName,bas.LName,Muni.UnitName,Muni.Abbreviation UnitAbbreviation,
                            faulty.IsEditAction,faulty.UpdatedOn,faulty.RemarksIds,faulty.FromRemark,faulty.ToRemark,bas.NameAsPerRecord,regi.Abbreviation RegimentalName,
                            CASE
                            WHEN LEFT(bas.ServiceNo, 2) LIKE '[A-Za-z][A-Za-z]' THEN
                            CONCAT(SUBSTRING(bas.ServiceNo, 1, 2), ' ', SUBSTRING(bas.ServiceNo, 3, LEN(bas.ServiceNo) - 2))
                            ELSE
                            bas.ServiceNo
                            END AS ModifiedServiceNo,
                            (select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(faulty.RemarksIds,','))) RemarksNameList
                            from TrnFaultyCard faulty
                            inner join MCategory mcat on mcat.CategoryId = faulty.CategoryId
                            inner join TrnICardRequest req on req.RequestId = faulty.RequestId
                            inner join TrnDomainMapping tdm on tdm.Id=req.TrnDomainMappingId
                            inner join BasicDetails bas on bas.BasicDetailId=req.BasicDetailId
                            inner join MRank ranks on ranks.RankId=bas.RankId
                            inner join MapUnit uni on uni.UnitMapId=bas.UnitId
                            inner join MUnit Muni on Muni.UnitId=uni.UnitId
                            inner join MApplyFor appl on appl.ApplyForId=bas.ApplyForId
                            left join MRegimental regi on regi.RegId=bas.RegimentalId";
                }
                else
                {
                    query = @"SELECT appl.Name ApplyFor,mcat.Name FaultyStage,req.RequestId,faulty.TrnFaultyCardId,bas.ServiceNo,ranks.RankAbbreviation RankName,bas.FName,bas.LName,Muni.UnitName,Muni.Abbreviation UnitAbbreviation,
                            faulty.IsEditAction,faulty.UpdatedOn,faulty.RemarksIds,faulty.FromRemark,faulty.ToRemark,bas.NameAsPerRecord,regi.Abbreviation RegimentalName,
                            CASE
                            WHEN LEFT(bas.ServiceNo, 2) LIKE '[A-Za-z][A-Za-z]' THEN
                            CONCAT(SUBSTRING(bas.ServiceNo, 1, 2), ' ', SUBSTRING(bas.ServiceNo, 3, LEN(bas.ServiceNo) - 2))
                            ELSE
                            bas.ServiceNo
                            END AS ModifiedServiceNo,
                            (select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(faulty.RemarksIds,','))) RemarksNameList
                            from TrnFaultyCard faulty
                            inner join MCategory mcat on mcat.CategoryId = faulty.CategoryId
                            inner join TrnICardRequest req on req.RequestId = faulty.RequestId
                            inner join TrnDomainMapping tdm on tdm.Id=req.TrnDomainMappingId and tdm.UnitId=@MapUnitId
                            inner join BasicDetails bas on bas.BasicDetailId=req.BasicDetailId
                            inner join MRank ranks on ranks.RankId=bas.RankId
                            inner join MapUnit uni on uni.UnitMapId=bas.UnitId
                            inner join MUnit Muni on Muni.UnitId=uni.UnitId
                            inner join MApplyFor appl on appl.ApplyForId=bas.ApplyForId
                            left join MRegimental regi on regi.RegId=bas.RegimentalId";
                }

                using (var connection = _contextDP.CreateConnection())
                {
                    var allrecord = await connection.QueryAsync<DTOFaultyCardListResponse>(query , new { MapUnitId });
                    return allrecord.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "FaultyCardDB->GetAllFaulty");
                return null;
            }

        }
        public async Task<DTOFaultyCardSaveResponse> SaveFaultyCard(DTOFaultyCardRequest dTO)
        {
            #region Old Code
            //DTOFaultyCardSaveResponse saveResponse = new DTOFaultyCardSaveResponse();
            //using var transaction_ = _context.Database.BeginTransaction();
            //try
            //{
            //    if (dTO.TrnFaultyCardId > 0)
            //    {
            //        TrnFaultyCard? trnFaultyCard = await _context.TrnFaultyCard.FindAsync(dTO.TrnFaultyCardId);
            //        if (trnFaultyCard != null)
            //        {
            //            trnFaultyCard.ToRemark = dTO.ToRemark;
            //            saveResponse.Id = trnFaultyCard.TrnFaultyCardId.ToString();
            //            saveResponse.CurrentTime = dTO.UpdatedOn ?? DateTime.Now;
            //            saveResponse.Result = true;
            //            saveResponse.Message = "Data Updated";
            //        }
            //        else
            //        {
            //            saveResponse.Result = false;
            //            saveResponse.Message = "Something went wrong or Invalid Input!";
            //        }
            //        return saveResponse;
            //    }
            //    else
            //    {
            //        MTrnICardRequest? mTrnICardRequest = await _context.TrnICardRequest.FindAsync(dTO.RequestId);
            //        if(mTrnICardRequest != null)
            //        {
            //            mTrnICardRequest.FlagForFaulty = true;
            //            _context.TrnICardRequest.Update(mTrnICardRequest);
            //            await _context.SaveChangesAsync();
            //        }

            //        TrnFaultyCard trnFaultyCard = new TrnFaultyCard();
            //        trnFaultyCard.TrnFaultyCardId = 0;
            //        trnFaultyCard.RemarksIds = dTO.RemarksIds;
            //        trnFaultyCard.FromRemark = dTO.FromRemark;
            //        trnFaultyCard.ToRemark = dTO.ToRemark ?? null;
            //        trnFaultyCard.CategoryId = dTO.CategoryId;
            //        trnFaultyCard.RequestId = dTO.RequestId;
            //        trnFaultyCard.UserId = dTO.UserId;
            //        trnFaultyCard.IsActive = dTO.IsActive;
            //        trnFaultyCard.Updatedby = dTO.Updatedby;
            //        trnFaultyCard.UpdatedOn = dTO.UpdatedOn;
            //        trnFaultyCard.IsEditAction = false;
            //        await _context.TrnFaultyCard.AddAsync(trnFaultyCard);
            //        await _context.SaveChangesAsync();

            //        transaction_.Commit();
            //        saveResponse.Id= trnFaultyCard.TrnFaultyCardId.ToString();
            //        saveResponse.CurrentTime = dTO.UpdatedOn ?? DateTime.Now;
            //        saveResponse.Result = true;
            //        saveResponse.Message = "Data has been saved";
            //        return saveResponse;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    transaction_.Rollback();
            //    _logger.LogError(1006, ex, "Exception");
            //    saveResponse.Result = false;
            //    saveResponse.Message = ex.Message;
            //    return saveResponse;
            //}
            #endregion
            DTOFaultyCardSaveResponse saveResponse = new DTOFaultyCardSaveResponse();
            var (db, transaction) = _contextDP.CreateConnectionWithTransaction();
            string insert_update_Sql = "";
            string query2 = "";
            string query3 = "";
            var parameters = new DynamicParameters();
            try
            {
                if (dTO.TrnFaultyCardId > 0)
                {
                    insert_update_Sql = @"UPDATE TrnFaultyCard set ToRemark = @ToRemark,IsEditAction=1 WHERE TrnFaultyCardId=@TrnFaultyCardId";

                    saveResponse.Message = "Data Updated";
                }
                else
                {
                    insert_update_Sql = @"INSERT INTO TrnFaultyCard(RemarksIds,FromRemark,ToRemark,CategoryId,RequestId,IsActive,UserId,Updatedby,UpdatedOn,IsEditAction)
                                            OUTPUT INSERTED.TrnFaultyCardId
                                            VALUES(@RemarksIds,@FromRemark,@ToRemark,@CategoryId,@RequestId,@IsActive,@UserId,@Updatedby,@UpdatedOn,@IsEditAction)";
                    
                    saveResponse.Message = "Data has been saved";

                }
                //Accept
                if (dTO.Choice == 2)
                {
                    query2 = @"UPDATE TrnStepCounter set StepId = 4 where RequestId=@RequestId ";
                    await db.ExecuteAsync(query2, new { dTO.RequestId }, transaction: transaction);

                }
                //Reject
                else if (dTO.Choice == 3)
                {
                    query3 = @"UPDATE AFSAC2.dbo.XmlFilesFwdLog SET XmlFiles='' WHERE RequestId=@RequestId";
                    await db.ExecuteAsync(query3, new { dTO.RequestId }, transaction: transaction);

                    query2 = @"UPDATE TrnStepCounter set StepId = 9 where RequestId=@RequestId ";
                    await db.ExecuteAsync(query2, new { dTO.RequestId }, transaction: transaction);
                }

                parameters.Add("@TrnFaultyCardId", dTO.TrnFaultyCardId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@RemarksIds", dTO.RemarksIds, DbType.String, ParameterDirection.Input, 100);
                parameters.Add("@FromRemark", dTO.FromRemark, DbType.String, ParameterDirection.Input,100);
                parameters.Add("@ToRemark", dTO.ToRemark, DbType.String, ParameterDirection.Input, 100);
                parameters.Add("@CategoryId", dTO.CategoryId, DbType.Byte, ParameterDirection.Input);
                parameters.Add("@RequestId", dTO.RequestId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@IsActive", dTO.IsActive, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@UserId", dTO.UserId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Updatedby", dTO.Updatedby, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@UpdatedOn", dTO.UpdatedOn, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@IsEditAction", dTO.IsActive, DbType.Boolean, ParameterDirection.Input);

                var Id = await db.QuerySingleAsync<int>(insert_update_Sql, parameters, transaction: transaction);

                string query1 = @" UPDATE TrnICardRequest set FlagForFaulty = 1 where RequestId=@RequestId ";
                await db.ExecuteAsync(query1, new { dTO.RequestId }, transaction: transaction);

                // Commit the transaction if all operations succeed
                transaction.Commit();
                saveResponse.Id = Id.ToString();
                saveResponse.CurrentTime = dTO.UpdatedOn ?? DateTime.Now;
                saveResponse.Result = true;
                return saveResponse;
            }
            catch (Exception ex)
            {
                // Rollback the transaction if any operation fails
                transaction.Rollback();
                _logger.LogError(1001, ex, "FaultyCardDB->SaveFaultyCard");
                saveResponse.Result = false;
                saveResponse.Message = ex.Message;
                return saveResponse;
            }
            finally
            {
                // Dispose of the connection
                db.Dispose();
            }
        }
    }
}
