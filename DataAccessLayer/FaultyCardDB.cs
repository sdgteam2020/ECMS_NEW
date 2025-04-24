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
                return await _context.TrnFaultyCard.AnyAsync(f => f.RequestId == RequestId && f.IsComplete == false);
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
                    query = @"SELECT appl.Name ApplyFor,mcat.Name FaultyStage,mcat.CategoryId,req.RequestId,faulty.TrnFaultyCardId,bas.ServiceNo,ranks.RankAbbreviation RankName,bas.FName,bas.LName,Muni.UnitName,Muni.Abbreviation UnitAbbreviation,
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
                            left join MRegimental regi on regi.RegId=bas.RegimentalId
                            order by faulty.TrnFaultyCardId desc";
                }
                else
                {
                    query = @"SELECT appl.Name ApplyFor,mcat.Name FaultyStage,mcat.CategoryId,req.RequestId,faulty.TrnFaultyCardId,bas.ServiceNo,ranks.RankAbbreviation RankName,bas.FName,bas.LName,Muni.UnitName,Muni.Abbreviation UnitAbbreviation,
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
                            left join MRegimental regi on regi.RegId=bas.RegimentalId
                            order by faulty.TrnFaultyCardId desc";
                }

                using (var connection = _contextDP.CreateConnection())
                {
                    var allrecordList = await connection.QueryAsync<DTOFaultyCardListResponse>(query , new { MapUnitId });
                    var allrecord = (from e in allrecordList
                                         select new DTOFaultyCardListResponse()
                                         {
                                             EncryptedId = protector.Protect(e.TrnFaultyCardId.ToString()),
                                             NameAsPerRecord = e.NameAsPerRecord,
                                             FName=e.FName,
                                             LName=e.LName,
                                             ServiceNo=e.ServiceNo,
                                             ModifiedServiceNo=e.ModifiedServiceNo,
                                             UnitName=e.UnitName,
                                             UnitAbbreviation=e.UnitAbbreviation,
                                             RankName=e.RankName,
                                             ArmedName=e.ArmedName,
                                             RequestId=e.RequestId,
                                             UpdatedOn=e.UpdatedOn,
                                             ApplyFor=e.ApplyFor,
                                             TrnFaultyCardId=e.TrnFaultyCardId,
                                             RemarksIds=e.RemarksIds,
                                             RemarksNameList=e.RemarksNameList,
                                             FromRemark = e.FromRemark,
                                             ToRemark = e.ToRemark, 
                                             CategoryId=e.CategoryId,
                                             FaultyStage= e.FaultyStage,
                                             IsEditAction=e.IsEditAction,
                                         }).ToList();
                    return allrecord.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "FaultyCardDB->GetAllFaulty");
                return null;
            }

        }
        public async Task<DTOFaultyCardListResponse?> GetTrnFaultyCardDetail(int TrnFaultyCardId)
        {
            try
            {
                string query = @"SELECT appl.Name ApplyFor,mcat.Name FaultyStage,mcat.CategoryId,req.RequestId,faulty.TrnFaultyCardId,bas.ServiceNo,ranks.RankAbbreviation RankName,bas.FName,bas.LName,Muni.UnitName,Muni.Abbreviation UnitAbbreviation,
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
                                left join MRegimental regi on regi.RegId=bas.RegimentalId
                                WHERE faulty.TrnFaultyCardId = @TrnFaultyCardId";


                using (var connection = _contextDP.CreateConnection())
                {
                    var allrecord = await connection.QueryAsync<DTOFaultyCardListResponse>(query, new { TrnFaultyCardId });
                    return allrecord.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "FaultyCardDB->GetTrnFaultyCardDetail");
                return null;
            }

        }
        public async Task<DTOFaultyCardSaveResponse> SaveFaultyCard(DTOFaultyCardRequest dTO, MTrnFwd? mTrnFwd)
        {
            DTOFaultyCardSaveResponse saveResponse = new DTOFaultyCardSaveResponse();
            var (db, transaction) = _contextDP.CreateConnectionWithTransaction();
            string insert = "";
            string update = "";
            string query2 = "";
            string query3 = "";

            try
            {
                if (dTO.TrnFaultyCardId > 0)
                {
                    update = @"UPDATE TrnFaultyCard set ToRemark = @ToRemark,IsEditAction = @IsEditAction,IsComplete = @IsComplete WHERE TrnFaultyCardId=@TrnFaultyCardId";
                    
                    var parameters = new DynamicParameters();
                    parameters.Add("@TrnFaultyCardId", dTO.TrnFaultyCardId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@ToRemark", dTO.ToRemark, DbType.String, ParameterDirection.Input, 100);
                    parameters.Add("@IsEditAction", dTO.IsEditAction, DbType.Boolean, ParameterDirection.Input);
                    parameters.Add("@IsComplete", dTO.IsComplete, DbType.Boolean, ParameterDirection.Input);

                    await db.ExecuteAsync(update, parameters, transaction: transaction);
                    
                    saveResponse.Id = dTO.TrnFaultyCardId.ToString();
                    saveResponse.Message = "Data Updated";
                }
                else
                {
                    insert = @"INSERT INTO TrnFaultyCard(RemarksIds,FromRemark,ToRemark,CategoryId,RequestId,IsActive,UserId,Updatedby,UpdatedOn,IsEditAction,TrnFwdId,IsComplete)
                                OUTPUT INSERTED.TrnFaultyCardId
                                VALUES(@RemarksIds,@FromRemark,@ToRemark,@CategoryId,@RequestId,@IsActive,@UserId,@Updatedby,@UpdatedOn,@IsEditAction,@TrnFwdId,@IsComplete)";
                    
                    var parameters = new DynamicParameters();
                    parameters.Add("@TrnFaultyCardId", dTO.TrnFaultyCardId, DbType.Int32, ParameterDirection.Output);
                    parameters.Add("@RemarksIds", dTO.RemarksIds, DbType.String, ParameterDirection.Input, 100);
                    parameters.Add("@FromRemark", dTO.FromRemark, DbType.String, ParameterDirection.Input, 100);
                    parameters.Add("@ToRemark", dTO.ToRemark, DbType.String, ParameterDirection.Input, 100);
                    parameters.Add("@CategoryId", dTO.CategoryId, DbType.Byte, ParameterDirection.Input);
                    parameters.Add("@RequestId", dTO.RequestId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@IsActive", dTO.IsActive, DbType.Boolean, ParameterDirection.Input);
                    parameters.Add("@UserId", dTO.UserId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Updatedby", dTO.Updatedby, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@UpdatedOn", dTO.UpdatedOn, DbType.DateTime, ParameterDirection.Input);
                    parameters.Add("@IsEditAction", dTO.IsEditAction, DbType.Boolean, ParameterDirection.Input);
                    parameters.Add("@TrnFwdId", dTO.TrnFwdId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@IsComplete", dTO.IsComplete, DbType.Boolean, ParameterDirection.Input);

                    var Id = await db.QuerySingleAsync<int>(insert, parameters, transaction: transaction);
                    saveResponse.Id = Id.ToString();
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
                    if (mTrnFwd != null)
                    {
                        insert = @"INSERT INTO TrnFwds(RequestId,ToUserId,FromUserId,FromAspNetUsersId,ToAspNetUsersId,UnitId,Remark,TypeId,IsComplete,IsActive,Updatedby,UpdatedOn,RemarksIds,FwdStatusId,StepId)
                                OUTPUT INSERTED.TrnFwdId
                                VALUES(@RequestId,@ToUserId,@FromUserId,@FromAspNetUsersId,@ToAspNetUsersId,@UnitId,@Remark,@TypeId,@IsComplete,@IsActive,@Updatedby,@UpdatedOn,@RemarksIds,@FwdStatusId,@StepId)";
                        var parameters = new DynamicParameters();
                        parameters.Add("@TrnFwdId", mTrnFwd.TrnFwdId, DbType.Int32, ParameterDirection.Output);
                        parameters.Add("@RequestId", mTrnFwd.RequestId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@ToUserId", mTrnFwd.ToUserId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@FromUserId", mTrnFwd.FromUserId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@FromAspNetUsersId", mTrnFwd.FromAspNetUsersId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@ToAspNetUsersId", mTrnFwd.ToAspNetUsersId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@UnitId", mTrnFwd.UnitId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@Remark", mTrnFwd.Remark, DbType.String, ParameterDirection.Input, 100);
                        parameters.Add("@TypeId", mTrnFwd.TypeId, DbType.Byte, ParameterDirection.Input);
                        parameters.Add("@IsComplete", mTrnFwd.IsComplete, DbType.Boolean, ParameterDirection.Input);
                        parameters.Add("@IsActive", mTrnFwd.IsActive, DbType.Boolean, ParameterDirection.Input);
                        parameters.Add("@Updatedby", mTrnFwd.Updatedby, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@UpdatedOn", mTrnFwd.UpdatedOn, DbType.DateTime, ParameterDirection.Input);
                        parameters.Add("@RemarksIds", mTrnFwd.RemarksIds, DbType.String, ParameterDirection.Input, 100);
                        parameters.Add("@FwdStatusId", mTrnFwd.FwdStatusId, DbType.Byte, ParameterDirection.Input);
                        parameters.Add("@StepId", mTrnFwd.StepId, DbType.Byte, ParameterDirection.Input);
                        
                        var Id = await db.QuerySingleAsync<int>(insert, parameters, transaction: transaction);
                    }
                    query3 = @"UPDATE AFSAC2.dbo.XmlFilesFwdLog SET XmlFiles='' WHERE RequestId=@RequestId";
                    await db.ExecuteAsync(query3, new { dTO.RequestId }, transaction: transaction);

                    query2 = @"UPDATE TrnStepCounter set StepId = 9 where RequestId=@RequestId ";
                    await db.ExecuteAsync(query2, new { dTO.RequestId }, transaction: transaction);
                }


                // Commit the transaction if all operations succeed
                transaction.Commit();
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
