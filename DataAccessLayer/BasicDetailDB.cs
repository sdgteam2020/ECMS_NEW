using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response.User;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Logger;
using Dapper;
using Azure.Core;
using DataTransferObject.Constants;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain;
using DataTransferObject.ViewModels;
using DataTransferObject.Response;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Dapper.SqlMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace DataAccessLayer
{
    public class BasicDetailDB : GenericRepositoryDL<BasicDetail>, IBasicDetailDB
    {
        protected readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly IDataProtector protector;
        private readonly ILogger<BasicDetailDB> _logger;
        public BasicDetailDB(ApplicationDbContext context, DapperContext contextDP, IDataProtectionProvider dataProtectionProvider, ILogger<BasicDetailDB> logger, DataProtectionPurposeStrings dataProtectionPurposeStrings) : base(context)
        {
            _context = context;
            _contextDP=contextDP;
            _logger = logger;
            // Pass the purpose string as a parameter
            this.protector = dataProtectionProvider.CreateProtector(
                dataProtectionPurposeStrings.AFSACIdRouteValue);
        }
        public async Task<bool> SaveBasicDetailsWithAll(BasicDetail Data, MTrnAddress address, MTrnUpload trnUpload, MTrnIdentityInfo mTrnIdentityInfo, MTrnICardRequest mTrnICardRequest, MStepCounter mStepCounter)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                if (Data.BasicDetailId == 0)
                {
                    _context.BasicDetails.Add(Data);
                   await _context.SaveChangesAsync();
                    int BasicDetailId = Data.BasicDetailId;
                    address.BasicDetailId = BasicDetailId;
                    _context.TrnAddress.Add(address);
                    await _context.SaveChangesAsync();
                    trnUpload.BasicDetailId = BasicDetailId;
                    _context.TrnUpload.Add(trnUpload);
                    await _context.SaveChangesAsync();
                    mTrnIdentityInfo.BasicDetailId = BasicDetailId;
                    _context.TrnIdentityInfo.Add(mTrnIdentityInfo);
                    await _context.SaveChangesAsync();
                    mTrnICardRequest.BasicDetailId = BasicDetailId;
                    _context.TrnICardRequest.Add(mTrnICardRequest);
                    await _context.SaveChangesAsync();
                    mStepCounter.RequestId = mTrnICardRequest.RequestId;
                    _context.TrnStepCounter.Add(mStepCounter);

                    await _context.SaveChangesAsync();

                    transaction.Commit();
                    return true;
                }
                else
                {
                   
                    address.BasicDetailId = Data.BasicDetailId;
                    trnUpload.BasicDetailId = Data.BasicDetailId;
                    mTrnIdentityInfo.BasicDetailId = Data.BasicDetailId;

                    _context.Update(address);
                    await _context.SaveChangesAsync();
                    _context.Update(trnUpload);
                    await _context.SaveChangesAsync();
                    _context.Update(mTrnIdentityInfo);
                    await _context.SaveChangesAsync();

                    _context.Entry(Data).State = EntityState.Modified;
                    _context.Update(Data);
                    await _context.SaveChangesAsync();

                    transaction.Commit();
                    return true;
                }
                //do other things, then commit or rollback
               
               
            }
           catch (Exception ex) {
                transaction.Rollback();
                return false; 
            }
            

            
        }
        public async Task<BasicDetail?> FindServiceNo(string ServiceNo)
        {
            string query = "Select * from BasicDetails where ServiceNo = @ServiceNo ";
            using (var connection = _contextDP.CreateConnection())
            {
                BasicDetail basicDetail = await connection.QuerySingleOrDefaultAsync<BasicDetail>(query, new { ServiceNo });
                if(basicDetail!=null)
                {
                    return basicDetail;
                }
                else
                {
                    return null;
                }   
            }

        }
        public async Task<List<DTOSmartSearch>> SearchAllServiceNo(string ServiceNo,int AspNetUsersId)
        {
            string query = "Select basi.BasicDetailId,Name,ServiceNo,PhotoImagePath Image "+
                           " from BasicDetails basi "+
                           " inner join TrnICardRequest req on req.BasicDetailId=basi.BasicDetailId and req.Status=0"+
                           " inner join TrnDomainMapping map on map.Id=req.TrnDomainMappingId and map.AspNetUsersId=@AspNetUsersId" +
                           " inner join TrnUpload trnu on basi.BasicDetailId=trnu.BasicDetailId where ServiceNo like @ServiceNo ";
            
            ServiceNo = "%" + ServiceNo.Replace("[", "[[]").Replace("%", "[%]") + "%";
            using (var connection = _contextDP.CreateConnection())
            {
                var basicDetail = await connection.QueryAsync<DTOSmartSearch>(query, new { AspNetUsersId,ServiceNo });
                if (basicDetail != null)
                {
                    return basicDetail.ToList();
                }
                else
                {
                    return null;
                }
            }

        }
        public async Task<List<DTOICardTypeRequest>> GetAllICardType()
        {
            string query = "Select * from MICardType";
            using (var connection = _contextDP.CreateConnection())
            {
                var ICardTypeList = await connection.QueryAsync<DTOICardTypeRequest>(query);
                var allrecord = (from e in ICardTypeList
                                 select new DTOICardTypeRequest()
                                 {
                                     TypeId = e.TypeId,
                                     EncryptedId = protector.Protect(e.TypeId.ToString()),
                                     Name=e.Name,
                                 }).ToList();
                return await Task.FromResult(allrecord);
            }
        }

        public async Task<List<BasicDetailVM>> GetALLForIcardSttaus(int UserId, int stepcount, int TypeId, int apply)
        {
            #region old code write by kapoor sir
            //int? applyfor = 0;
            //if (apply == 0) applyfor = null; else applyfor = apply;

            //string query = "";

            //if (stepcount == 0)//////For all record
            //{
            //    query = "SELECT B.UnitId,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.Name ICardType,trnicrd.RequestId,fwd.Remark,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId,Afor.Name ApplyFor,Afor.ApplyForId ,trnicrd.TrackingId,ran.RankAbbreviation RankName,ISNULL(Postout.Id,0) IsPosting FROM BasicDetails B " +
            //            "inner join MRank ran on ran.RankId=B.RankId " +
            //            "inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId " +
            //            "inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
            //            "inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId " +
            //            "inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
            //            "inner join TrnDomainMapping map on map.Id= trnicrd.TrnDomainMappingId " +
            //            "inner join UserProfile pr on pr.UserId = map.UserId " +
            //            "left join TrnFwds fwd on fwd.ToAspNetUsersId= map.AspNetUsersId and fwd.IsComplete=0 and fwd.RequestId=trnicrd.RequestId " +
            //            "left join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId " +
            //            "left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId and trnicrd.Status=0 " +
            //            "WHERE map.AspNetUsersId = @UserId and Afor.ApplyForId=ISNULL(@applyfor,Afor.ApplyForId) ORDER BY B.UpdatedOn DESC";

            //}
            //else if (stepcount == 1)//////For Draft
            //{
            //    query = "SELECT B.UnitId,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.Name ICardType,trnicrd.RequestId,fwd.Remark,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId,Afor.Name ApplyFor,Afor.ApplyForId ,trnicrd.TrackingId,ran.RankAbbreviation RankName,ISNULL(Postout.Id,0) IsPosting FROM BasicDetails B " +
            //            "inner join MRank ran on ran.RankId=B.RankId " +
            //            "inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId " +
            //            "inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
            //            "inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId " +
            //            "inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
            //            "inner join TrnDomainMapping map on map.Id= trnicrd.TrnDomainMappingId " +
            //            "inner join UserProfile pr on pr.UserId = map.UserId " +
            //            "left join TrnFwds fwd on fwd.ToAspNetUsersId= map.AspNetUsersId and fwd.IsComplete=0 and fwd.RequestId=trnicrd.RequestId " +
            //            "left join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId " +
            //            "left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId and trnicrd.Status=0 " +
            //            "WHERE map.AspNetUsersId = @UserId and Afor.ApplyForId=ISNULL(@applyfor,Afor.ApplyForId) and trnicrd.Status=0  and C.StepId = @stepcount  ORDER BY B.UpdatedOn DESC";

            //}
            //else if (stepcount == 777)//////For Completed   
            //{
            //    query = "SELECT B.UnitId,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.Name ICardType,trnicrd.RequestId,fwd.Remark,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId,Afor.Name ApplyFor,Afor.ApplyForId ,trnicrd.TrackingId,ran.RankAbbreviation RankName,ISNULL(Postout.Id,0) IsPosting FROM BasicDetails B " +
            //            " inner join MRank ran on ran.RankId=B.RankId " +
            //            "inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId " +
            //            "inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
            //            "inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId " +
            //            "inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
            //            "inner join TrnDomainMapping map on map.Id= trnicrd.TrnDomainMappingId " +
            //            "inner join UserProfile pr on pr.UserId = map.UserId " +
            //            "left join TrnFwds fwd on fwd.ToAspNetUsersId= map.AspNetUsersId and fwd.IsComplete=0 and fwd.RequestId=trnicrd.RequestId " +
            //            "left join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId " +
            //            "left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId and trnicrd.Status=0 " +
            //            "WHERE map.AspNetUsersId = @UserId and Afor.ApplyForId=ISNULL(@applyfor,Afor.ApplyForId) and trnicrd.Status = 1  ORDER BY B.UpdatedOn DESC";

            //}
            //else if (stepcount == 888)//////For Submitted
            //{
            //    query = "SELECT B.UnitId,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.Name ICardType,trnicrd.RequestId,fwd.Remark,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId,Afor.Name ApplyFor,Afor.ApplyForId ,trnicrd.TrackingId,ran.RankAbbreviation RankName,ISNULL(Postout.Id,0) IsPosting FROM BasicDetails B " +
            //            "inner join MRank ran on ran.RankId=B.RankId " +
            //            "inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId and trnicrd.Status=0 " +
            //            "inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
            //            "inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId " +
            //            "inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
            //            "inner join TrnDomainMapping map on map.Id= trnicrd.TrnDomainMappingId " +
            //            "inner join UserProfile pr on pr.UserId = map.UserId " +
            //            "left join TrnFwds fwd on fwd.ToAspNetUsersId= map.AspNetUsersId and fwd.IsComplete=0 and fwd.RequestId=trnicrd.RequestId " +
            //            "left join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId " +
            //            "left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId  " +
            //            "WHERE map.AspNetUsersId = @UserId and Afor.ApplyForId=ISNULL(@applyfor,Afor.ApplyForId) and C.StepId > 1  ORDER BY B.UpdatedOn DESC";

            //}
            //else if (stepcount == 5)
            //{
            //    query = " SELECT distinct B.UnitId,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId, ISNULL(fwd.FwdStatusId,0) IsFwdStatusId, Afor.Name ApplyFor,Afor.ApplyForId ,trnicrd.TrackingId,ran.RankAbbreviation RankName,ISNULL(Postout.Id,0) IsPosting" +
            //            " FROM BasicDetails B" +
            //            " inner join MRank ran on ran.RankId=B.RankId" +
            //            " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
            //            " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
            //            " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
            //            " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
            //            " left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId and trnicrd.Status=1 " +
            //            " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and Afor.ApplyForId=ISNULL(@applyfor,Afor.ApplyForId) and C.StepId = @stepcount where trnicrd.Status=1" +
            //            " inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId ";
            //}

            //else if (stepcount == 2 || stepcount == 3 || stepcount == 4 || stepcount == 5 || stepcount == 6)//IO
            //{
            //    //if(TypeId==2)
            //    {
            //        query = " SELECT distinct B.UnitId,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId, ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId ,trnicrd.TrackingId,ran.RankAbbreviation RankName,ISNULL(Postout.Id,0) IsPosting" +
            //                " FROM BasicDetails B" +
            //                " inner join MRank ran on ran.RankId=B.RankId" +
            //                " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
            //                " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
            //                " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
            //                " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
            //                " left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId and trnicrd.Status=0 " +
            //                " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and Afor.ApplyForId=ISNULL(@applyfor,Afor.ApplyForId) and C.StepId = @stepcount where trnicrd.Status=0" +
            //                " inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId ";
            //    }
            //    //    else if (TypeId == 3)
            //    //    {
            //    //        query = " SELECT distinct B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,Afor.Name ApplyFor,Afor.ApplyForId ,trnicrd.TrackingId,ran.RankAbbreviation RankName,ISNULL(Postout.Id,0) IsPosting" +
            //    //" FROM BasicDetails B" +
            //    //" inner join MRank ran on ran.RankId=B.RankId" +
            //    //" inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
            //    //" inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
            //    //" inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
            //    //" inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
            //    //" left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId and trnicrd.Status=0 " +
            //    //" inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and Afor.ApplyForId=ISNULL(@applyfor,Afor.ApplyForId) and C.StepId = @stepcount where trnicrd.Status=0";
            //    //    }


            //}
            //else if (stepcount == 7 || stepcount == 8 || stepcount == 9 || stepcount == 10)//Reject From IO
            //{

            //    query = "SELECT distinct B.UnitId,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId, ISNULL(fwd.FwdStatusId,0) IsFwdStatusId, Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId,ran.RankAbbreviation RankName,ISNULL(Postout.Id,0) IsPosting" +
            //            " FROM BasicDetails B" +
            //            " inner join MRank ran on ran.RankId=B.RankId" +
            //            " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
            //            " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
            //            " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
            //            " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
            //            " left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId and trnicrd.Status=0 " +
            //            " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.ToAspNetUsersId = @UserId and Afor.ApplyForId=ISNULL(@applyfor,Afor.ApplyForId) and fwd.Status=0 and C.StepId = @stepcount where trnicrd.Status=0" +
            //            " inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId ";
            //}
            //else if (stepcount == 999)//Reject From IO,MI11 and HQ 54
            //{

            //    query = "SELECT distinct B.UnitId,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId, ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId,ran.RankAbbreviation RankName,ISNULL(Postout.Id,0) IsPosting" +
            //            " FROM BasicDetails B" +
            //            " inner join MRank ran on ran.RankId=B.RankId" +
            //            " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
            //            " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
            //            " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
            //            " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
            //            " left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId and trnicrd.Status=0 " +
            //            " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.ToAspNetUsersId = @UserId and Afor.ApplyForId=ISNULL(@applyfor,Afor.ApplyForId) and fwd.Status=0 and C.StepId in (7,8,9,10) where trnicrd.Status=0" +
            //            " inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId ";
            //}

            //else if (stepcount == 4)///MI-11
            //{
            //    query = "SELECT B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.Step StepCounter,C.Id StepId,ty.Name ICardType,trnicrd.RequestId ,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId" +
            //                "FROM BasicDetails B " +
            //                "inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId " +
            //                " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
            //                "inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId " +
            //                "inner join MICardType ty on ty.TypeId = trnicrd.TypeId inner join UserProfile pro on pro.ArmyNo = B.ServiceNo " +
            //                "inner join MMappingProfile mpro on mpro.UserId = pro.UserId " +
            //                "inner join TrnFwds FWD ON FWD.RequestId = c.RequestId " +
            //                "inner join MUnit mUNI ON mUNI.UnitId = FWD.SusNo " +
            //                "WHERE mUNI.UnitId = @UserId and trnicrd.Status=0 AND C.Step = @stepcount ORDER BY B.UpdatedOn DESC";
            //}
            //else if (stepcount == 5)///Hq-54
            //{
            //    query = "SELECT B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.Step StepCounter,C.Id StepId,ty.Name ICardType,trnicrd.RequestId ,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId FROM BasicDetails B " +
            //               "inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId " +
            //               " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
            //               "inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId " +
            //               "inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
            //               "inner join UserProfile pro on pro.ArmyNo=B.ServiceNo " +
            //               "inner join TrnFwds FWD ON FWD.RequestId = c.RequestId " +
            //               "inner join MMappingProfile mpro on mpro.UserId = pro.UserId" +
            //               " WHERE FWD.ToUserId = @UserId and trnicrd.Status=0 and C.Step = 5 ORDER BY B.UpdatedOn DESC";
            //}

            //else if (stepcount == 33)//Reject From GsO
            //{
            //    stepcount = 1;
            //    query = "SELECT distinct B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId" +
            //    " FROM BasicDetails B" +
            //    " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
            //    " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
            //    " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
            //    " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
            //    " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.ToAspNetUsersId = @UserId and fwd.TypeId=@TypeId and C.StepId = @stepcount and trnicrd.Status=0";
            //}
            #endregion end old code write by kapoor sir

            int? applyfor = 0;
            if (apply == 0) applyfor = null; else applyfor = apply;

            string query = "";

            if (stepcount == 0)//////For all record
            {
                query = "SELECT munit.UnitName,B.UnitId,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.Name ICardType,trnicrd.RequestId,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId,fwd.Remark,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId,Afor.Name ApplyFor,Afor.ApplyForId ,trnicrd.TrackingId,ran.RankAbbreviation RankName,ISNULL(Postout.Id,0) IsPosting FROM BasicDetails B " +
                        "inner join MRank ran on ran.RankId=B.RankId "  +
                        "inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                        "inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                        "inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId " +
                        "inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                        "inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId " +
                        "inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
                        "inner join TrnDomainMapping map on map.Id= trnicrd.TrnDomainMappingId " +
                        "inner join UserProfile pr on pr.UserId = map.UserId " +
                        "left join TrnFwds fwd on fwd.FromAspNetUsersId= map.AspNetUsersId and fwd.IsComplete=0 and fwd.RequestId=trnicrd.RequestId " +
                        "left join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId " +
                        "left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId and trnicrd.Status=0 " +
                        "WHERE map.AspNetUsersId = @UserId and Afor.ApplyForId=ISNULL(@applyfor,Afor.ApplyForId) ORDER BY B.UpdatedOn DESC";

            }
            else if (stepcount == 1)//////For Draft
            {
                query = "SELECT munit.UnitName,B.UnitId,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.Name ICardType,trnicrd.RequestId,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId,fwd.Remark,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId,Afor.Name ApplyFor,Afor.ApplyForId ,trnicrd.TrackingId,ran.RankAbbreviation RankName,ISNULL(Postout.Id,0) IsPosting FROM BasicDetails B " +
                        "inner join MRank ran on ran.RankId=B.RankId " +
                        "inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                        "inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                        "inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId " +
                        "inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                        "inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId " +
                        "inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
                        "inner join TrnDomainMapping map on map.Id= trnicrd.TrnDomainMappingId " +
                        "inner join UserProfile pr on pr.UserId = map.UserId " +
                        "left join TrnFwds fwd on fwd.FromAspNetUsersId= map.AspNetUsersId and fwd.IsComplete=0 and fwd.RequestId=trnicrd.RequestId " +
                        "left join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId " +
                        "left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId and trnicrd.Status=0 " +
                        "WHERE map.AspNetUsersId = @UserId and Afor.ApplyForId=ISNULL(@applyfor,Afor.ApplyForId) and trnicrd.Status=0  and C.StepId = @stepcount  ORDER BY B.UpdatedOn DESC";

            }
            else if (stepcount == 777)//////For Completed   
            {
                query = "SELECT munit.UnitName,B.UnitId,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.Name ICardType,trnicrd.RequestId,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId,fwd.Remark,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId,Afor.Name ApplyFor,Afor.ApplyForId ,trnicrd.TrackingId,ran.RankAbbreviation RankName,ISNULL(Postout.Id,0) IsPosting FROM BasicDetails B " +
                        "inner join MRank ran on ran.RankId=B.RankId " +
                        "inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                        "inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                        "inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId " +
                        "inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                        "inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId " +
                        "inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
                        "inner join TrnDomainMapping map on map.Id= trnicrd.TrnDomainMappingId " +
                        "inner join UserProfile pr on pr.UserId = map.UserId " +
                        "inner join TrnFwds fwd on fwd.FromAspNetUsersId= map.AspNetUsersId and fwd.IsComplete=1 and fwd.RequestId=trnicrd.RequestId " +
                        "inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId " +
                        "left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId and trnicrd.Status=0 " +
                        "WHERE map.AspNetUsersId = @UserId and Afor.ApplyForId=ISNULL(@applyfor,Afor.ApplyForId) and trnicrd.Status = 1  ORDER BY B.UpdatedOn DESC";

            }
            else if (stepcount == 888)//////For Submitted
            {
                query = "SELECT distinct munit.UnitName,B.UnitId,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.Name ICardType,trnicrd.RequestId,Afor.Name ApplyFor,Afor.ApplyForId ,trnicrd.TrackingId,ran.RankAbbreviation RankName,ISNULL(Postout.Id,0) IsPosting FROM BasicDetails B " +
                        "inner join MRank ran on ran.RankId=B.RankId " +
                        "inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                        "inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                        "inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId and trnicrd.Status=0 " +
                        "inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                        "inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId " +
                        "inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
                        "inner join TrnDomainMapping map on map.Id= trnicrd.TrnDomainMappingId " +
                        "inner join UserProfile pr on pr.UserId = map.UserId " +
                        "left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId  " +
                        "WHERE map.AspNetUsersId = @UserId and Afor.ApplyForId=ISNULL(@applyfor,Afor.ApplyForId) and C.StepId > 1 ";

            }
            else if (stepcount == 5)
            {
                query = " SELECT distinct munit.UnitName,B.UnitId,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId, ISNULL(fwd.FwdStatusId,0) IsFwdStatusId, Afor.Name ApplyFor,Afor.ApplyForId ,trnicrd.TrackingId,ran.RankAbbreviation RankName,ISNULL(Postout.Id,0) IsPosting" +
                        " FROM BasicDetails B" +
                        " inner join MRank ran on ran.RankId=B.RankId" +
                        " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                        " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                        " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
                        " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                        " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
                        " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
                        " left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId and trnicrd.Status=1 " +
                        " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and Afor.ApplyForId=ISNULL(@applyfor,Afor.ApplyForId) and C.StepId = @stepcount " +
                        " inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId " +
                        " where trnicrd.Status=1 ";
            }

            else if (stepcount == 2 || stepcount == 3 || stepcount == 4 || stepcount == 5 || stepcount == 6)//IO
            {
               //if(TypeId==2)
                {
                    query = " SELECT distinct munit.UnitName,B.UnitId,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId, ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId ,trnicrd.TrackingId,ran.RankAbbreviation RankName,ISNULL(Postout.Id,0) IsPosting" +
                            " FROM BasicDetails B" +
                            " inner join MRank ran on ran.RankId=B.RankId" +
                            " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                            " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                            " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
                            " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                            " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
                            " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
                            " left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId and trnicrd.Status=0 " +
                            " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and Afor.ApplyForId=ISNULL(@applyfor,Afor.ApplyForId) and fwd.IsComplete=0 and C.StepId = @stepcount " +
                            " inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId " +
                            " where trnicrd.Status=0 ";
                }
            //    else if (TypeId == 3)
            //    {
            //        query = " SELECT distinct B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,Afor.Name ApplyFor,Afor.ApplyForId ,trnicrd.TrackingId,ran.RankAbbreviation RankName,ISNULL(Postout.Id,0) IsPosting" +
            //" FROM BasicDetails B" +
            //" inner join MRank ran on ran.RankId=B.RankId" +
            //" inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
            //" inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
            //" inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
            //" inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
            //" left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId and trnicrd.Status=0 " +
            //" inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and Afor.ApplyForId=ISNULL(@applyfor,Afor.ApplyForId) and C.StepId = @stepcount where trnicrd.Status=0";
            //    }


            }
            else if (stepcount == 7 || stepcount == 8 || stepcount == 9 || stepcount == 10 )//Reject From IO
            {

                query = "SELECT distinct munit.UnitName,B.UnitId,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId, ISNULL(fwd.TrnFwdId,0) IsTrnFwdId,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId, Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId,ran.RankAbbreviation RankName,ISNULL(Postout.Id,0) IsPosting" +
                        " FROM BasicDetails B" +
                        " inner join MRank ran on ran.RankId=B.RankId" +
                        " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                        " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                        " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
                        " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                        " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
                        " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
                        " left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId and trnicrd.Status=0 " +
                        " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.ToAspNetUsersId = @UserId and Afor.ApplyForId=ISNULL(@applyfor,Afor.ApplyForId) and fwd.FwdStatusId=3 " +
                        " inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId " +
                        " where trnicrd.Status=0 ";
            }
            else if (stepcount == 999)//Reject From IO,MI11 and HQ 54
            {

                query = "SELECT distinct munit.UnitName,B.UnitId,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId, ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId,ran.RankAbbreviation RankName,ISNULL(Postout.Id,0) IsPosting" +
                        " FROM BasicDetails B" +
                        " inner join MRank ran on ran.RankId=B.RankId" +
                        " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                        " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                        " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
                        " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                        " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
                        " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
                        " left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId and trnicrd.Status=0 " +
                        " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.ToAspNetUsersId = @UserId and Afor.ApplyForId=ISNULL(@applyfor,Afor.ApplyForId) and fwd.FwdStatusId=3 and C.StepId in (7,8,9,10) " +
                        " inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId "+
                        " where trnicrd.Status=0";
            }
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var BasicDetailList = await connection.QueryAsync<BasicDetailVM>(query, new { UserId, stepcount, TypeId, applyfor });
                    int sno = 1;
                    var allrecord = (from e in BasicDetailList
                                     select new BasicDetailVM()
                                     {
                                         BasicDetailId = e.BasicDetailId,
                                         EncryptedId = protector.Protect(e.BasicDetailId.ToString()),
                                         Sno = sno++,
                                         Name = e.Name,
                                         ServiceNo = e.ServiceNo,
                                         DOB = e.DOB,
                                         DateOfCommissioning = e.DateOfCommissioning,
                                         PermanentAddress = e.PermanentAddress,
                                         IsTrnFwdId = e.IsTrnFwdId,
                                         StepCounter = e.StepCounter,
                                         StepId = e.StepId,
                                         ICardType = e.ICardType,
                                         ApplyFor = e.ApplyFor,
                                         ApplyForId = e.ApplyForId,
                                         RequestId = e.RequestId,
                                         IsFwdStatusId = e.IsFwdStatusId,
                                         Remark = e.Remark,
                                         TrackingId = e.TrackingId,
                                         RankName = e.RankName,
                                         IsPosting = e.IsPosting,
                                         UnitName = e.UnitName,
                                         UnitId = e.UnitId
                                     }).ToList();
                    return await Task.FromResult(allrecord);

                }
            }
            catch(Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetALLForIcardSttaus");
                return null;
            }
        }
        public async Task<List<BasicDetailVM>> GetALLBasicDetail(int UserId,int stepcount, int TypeId, int applyForId)
        {
            #region old code write by Kapoor Sir
            //string query = "";

            //if (stepcount == 0 || stepcount == 1)//////For Fwd Record
            //{
            //    query = "SELECT B.UnitId,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.Name ICardType,trnicrd.RequestId,fwd.Remark,isnull(fwd.Status,1) Reject,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId,ran.RankAbbreviation RankName  FROM BasicDetails B " +
            //            " inner join MRank ran on ran.RankId=B.RankId " +
            //            "inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId " +
            //            "inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
            //            "inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId " +
            //            "inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
            //             "inner join TrnDomainMapping map on map.Id= trnicrd.TrnDomainMappingId " +
            //            "inner join UserProfile pr on pr.UserId = map.UserId " +
            //            "left join TrnFwds fwd on fwd.ToAspNetUsersId= map.AspNetUsersId and fwd.IsComplete=0 and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId) and fwd.RequestId=trnicrd.RequestId " +
            //            "WHERE map.AspNetUsersId = @UserId and trnicrd.Status=0 ORDER BY B.UpdatedOn DESC";

            //}
            //else if (stepcount == 2 || stepcount == 3 || stepcount == 4 || stepcount == 5 || stepcount == 6)//IO
            //{
            //    if (TypeId == 1)///For Icard Submit
            //    {
            //        query = " SELECT distinct B.UnitId,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId,ran.RankAbbreviation RankName" +
            //   " FROM BasicDetails B" +
            //   " inner join MRank ran on ran.RankId=B.RankId " +
            //   " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
            //   " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
            //   " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
            //   " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
            //   " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId) and fwd.TypeId=@stepcount and C.StepId = @stepcount and trnicrd.Status=0";
            //    }
            //    else if (TypeId == 2) //// For For Action
            //    {
            //        query = " SELECT distinct B.UnitId,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId,ran.RankAbbreviation RankName" +
            //   " FROM BasicDetails B" +
            //   " inner join MRank ran on ran.RankId=B.RankId" +
            //   " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
            //   " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
            //   " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
            //   " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
            //   " left join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.ToAspNetUsersId = @UserId and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId) and fwd.TypeId=@stepcount and C.StepId = @stepcount and trnicrd.Status=0";


            //    }
            //    else if (stepcount == 5 || stepcount == 6)///for exported data
            //    {
            //        query = " SELECT distinct B.UnitId,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,Afor.Name ApplyFor,Afor.ApplyForId,ran.RankAbbreviation RankName" +
            //" FROM BasicDetails B" +
            //" inner join MRank ran on ran.RankId=B.RankId" +
            //" inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
            //" inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
            //" inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
            //" inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
            //" inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.ToAspNetUsersId = @UserId and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId)  and trnicrd.Status=1";

            //    }
            //    // else if (stepcount == 5) //// For 
            //    // {
            //    //     query = " SELECT distinct B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId,ran.RankAbbreviation RankName" +
            //    //" FROM BasicDetails B" +
            //    //" inner join MRank ran on ran.RankId=B.RankId" +
            //    //" inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
            //    //" inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
            //    //" inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
            //    //" inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
            //    //" inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.ToAspNetUsersId = @UserId and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId) and fwd.TypeId=@stepcount and C.StepId = @stepcount and trnicrd.Status=0";


            //    // }
            //    else //if (TypeId == 3) //// For For Show
            //    {
            //        TypeId = stepcount - 1;
            //        //if (TypeId == 3) TypeId = 2;
            //        //if (TypeId == 4) TypeId = 3;
            //        //if (TypeId == 5) TypeId = 4;
            //        query = " SELECT distinct B.UnitId,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,Afor.Name ApplyFor,Afor.ApplyForId,ran.RankAbbreviation RankName" +
            //   " FROM BasicDetails B" +
            //   " inner join MRank ran on ran.RankId=B.RankId" +
            //   " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
            //   " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
            //   " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
            //   " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
            //   " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.ToAspNetUsersId = @UserId and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId) and fwd.TypeId=@TypeId and fwd.IsComplete=1 and trnicrd.Status=0";


            //    }
            //}
            //else if (stepcount == 7 || stepcount == 8 || stepcount == 9 || stepcount == 10)//Reject From IO
            //{

            //    query = "SELECT distinct B.UnitId,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId,isnull(fwd.Status,1) Reject ,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId,ran.RankAbbreviation RankName" +
            //    " FROM BasicDetails B" +
            //    " inner join MRank ran on ran.RankId=B.RankId" +
            //    " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
            //    " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
            //    " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
            //    " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
            //    " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId) and fwd.Status=0 where trnicrd.Status=0";
            //}
            //else if (stepcount == 22)//Reject From IO
            //{
            //    stepcount = 1;
            //    query = "SELECT distinct B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId,fwd.Remark,isnull(fwd.Status,1) Reject ,Afor.Name ApplyFor,Afor.ApplyForId" +
            //    " FROM BasicDetails B" +
            //    " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
            //    " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
            //    " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
            //    " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
            //    " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and fwd.TypeId=@TypeId and fwd.Status=0 and C.StepId = 1 and trnicrd.Status=0";
            //}
            //else if (TypeId == 2)////GSO
            //{
            //    query = "SELECT B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.Step StepCounter,C.Id StepId,ty.Name ICardType,trnicrd.RequestId  FROM BasicDetails B " +
            //               "inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId " +
            //               "inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId " +
            //               "inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
            //               "inner join UserProfile pro on pro.ArmyNo=B.ServiceNo " +
            //               "inner join MMappingProfile mpro on mpro.UserId=pro.UserId " +
            //               "WHERE  mpro.GSOId = @UserId and C.Step = @stepcount ORDER BY B.UpdatedOn DESC";
            //}
            //else if (stepcount == 4)///MI-11
            //{
            //    if (TypeId == 1) //// For For Action
            //    {
            //        query = " SELECT distinct B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId " +
            //   " FROM BasicDetails B" +
            //   " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
            //   " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
            //   " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
            //   " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.ToAspNetUsersId = @UserId and fwd.TypeId=@stepcount and C.StepId = @stepcount";


            //    } 
            //    else if (TypeId == 3)
            //    {
            //        query = "SELECT distinct B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId " +
            //      " FROM BasicDetails B" +
            //      " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
            //      " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
            //      " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
            //      " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.ToAspNetUsersId = @UserId and fwd.TypeId=3 and fwd.IsComplete=1";
            // }


            //    //query = "SELECT B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.Step StepCounter,C.Id StepId,ty.NameICardType,trnicrd.RequestId " + 
            //    //            "FROM BasicDetails B "+
            //    //            "inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId "+
            //    //            "inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId "+
            //    //            "inner join MICardType ty on ty.TypeId = trnicrd.TypeId inner join UserProfile pro on pro.ArmyNo = B.ServiceNo "+
            //    //            "inner join MMappingProfile mpro on mpro.UserId = pro.UserId "+
            //    //            "inner join TrnFwds FWD ON FWD.RequestId = c.RequestId "+
            //    //            "inner join MUnit mUNI ON mUNI.UnitId = FWD.SusNo "+
            //    //            "WHERE mUNI.UnitId = @UserId AND C.Step = @stepcount ORDER BY B.UpdatedOn DESC";
            //}
            //else if (stepcount == 5)///Hq-54
            //{
            //    query = "SELECT B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.Step StepCounter,C.Id StepId,ty.Name ICardType,trnicrd.RequestId  FROM BasicDetails B " +
            //               "inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId " +
            //               "inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId " +
            //               "inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
            //               "inner join UserProfile pro on pro.ArmyNo=B.ServiceNo " +
            //               "inner join TrnFwds FWD ON FWD.RequestId = c.RequestId " +
            //               "inner join MMappingProfile mpro on mpro.UserId = pro.UserId" +
            //               " WHERE FWD.ToUserId = @UserId and C.Step = 5 ORDER BY B.UpdatedOn DESC";
            //}

            //else if (stepcount == 33)//Reject From GsO
            //{
            //    stepcount = 1;
            //    query = "SELECT distinct B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,Afor.Name ApplyFor,Afor.ApplyForId" +
            //    " FROM BasicDetails B" +
            //    " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
            //    " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
            //    " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
            //    " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
            //    " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and fwd.TypeId=@TypeId and C.StepId = @stepcount and trnicrd.Status=0";
            //}

            #endregion end old code write by Kapoor Sir

            string query = "";

            if(stepcount == 0 || stepcount == 1)//////For Fwd Record
            {
                query = "SELECT munit.UnitName,B.UnitId,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId,C.Id StepId,ty.Name ICardType,trnicrd.RequestId,fwd.Remark,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId,ran.RankAbbreviation RankName  FROM BasicDetails B " +
                        "inner join MRank ran on ran.RankId=B.RankId " +
                        "inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                        "inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                        "inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId " +
                        "inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                        "inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId " +
                        "inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
                        "inner join TrnDomainMapping map on map.Id= trnicrd.TrnDomainMappingId " +
                        "inner join UserProfile pr on pr.UserId = map.UserId " +
                        "left join TrnFwds fwd on fwd.ToAspNetUsersId= map.AspNetUsersId and fwd.IsComplete=0 and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId) and fwd.RequestId=trnicrd.RequestId " +
                        "left join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId " +
                        "WHERE map.AspNetUsersId = @UserId and trnicrd.Status=0 ORDER BY B.UpdatedOn DESC";
                
            }
            else if (stepcount == 2 || stepcount == 3 || stepcount == 4 || stepcount == 5 || stepcount == 6)//IO
            {
                if(TypeId==1)///For Icard Submit
                {
                    query = " SELECT distinct munit.UnitName,B.UnitId,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId,ran.RankAbbreviation RankName" +
                           " FROM BasicDetails B" +
                           " inner join MRank ran on ran.RankId=B.RankId " +
                           " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                           " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                           " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
                           " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                           " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
                           " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
                           " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId) and fwd.TypeId=@stepcount and C.StepId = @stepcount and trnicrd.Status=0" +
                           " inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId ";
                }
                else if (stepcount == 3 && TypeId == 2 && applyForId ==2) //// For For Action
                {
                    query = " SELECT distinct munit.UnitName,B.UnitId,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId,ran.RankAbbreviation RankName" +
                            " FROM BasicDetails B" +
                            " inner join MRank ran on ran.RankId=B.RankId" +
                            " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                            " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                            " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
                            " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                            " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
                            " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
                            " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.ToAspNetUsersId = @UserId and fwd.IsComplete=0 and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId) and fwd.TypeId=@stepcount and C.StepId = @stepcount and trnicrd.Status=0" +
                            " inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId ";
                }
                else if (TypeId == 2) //// For For Action
                {
                    query = " SELECT distinct munit.UnitName,B.UnitId,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId,ran.RankAbbreviation RankName" +
                            " FROM BasicDetails B" +
                            " inner join MRank ran on ran.RankId=B.RankId" +
                            " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                            " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                            " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
                            " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                            " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
                            " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
                            " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.ToAspNetUsersId = @UserId and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId) and fwd.TypeId=@stepcount and fwd.IsComplete = 0 and C.StepId = @stepcount and trnicrd.Status=0" +
                            " inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId ";
                }
                else if(stepcount == 5 || stepcount == 6)///for exported data
                {
                    query = " SELECT distinct munit.UnitName,B.UnitId,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,ran.RankAbbreviation RankName" +
                            " FROM BasicDetails B" +
                            " inner join MRank ran on ran.RankId=B.RankId" +
                            " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                            " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                            " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
                            " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                            " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
                            " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
                            " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.ToAspNetUsersId = @UserId and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId)  and trnicrd.Status=1" +
                            " inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId ";

                }
                else //if (TypeId == 3) //// For For Show
                {
                    TypeId = stepcount - 1;
                    //if (TypeId == 3) TypeId = 2;
                    //if (TypeId == 4) TypeId = 3;
                    //if (TypeId == 5) TypeId = 4;
                    query = " SELECT distinct munit.UnitName,B.UnitId,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,ran.RankAbbreviation RankName" +
                            " FROM BasicDetails B" +
                            " inner join MRank ran on ran.RankId=B.RankId" +
                            " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                            " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                            " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
                            " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                            " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
                            " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
                            " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.ToAspNetUsersId = @UserId and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId) and fwd.TypeId=@TypeId and fwd.IsComplete=1 " +
                            " inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId ";


                }
            }
            else if (stepcount == 7 || stepcount == 8 || stepcount == 9 || stepcount == 10 )//Reject From IO
            {

                query = "SELECT distinct munit.UnitName,B.UnitId,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,fwd.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId,ran.RankAbbreviation RankName" +
                        " FROM BasicDetails B" +
                        " inner join MRank ran on ran.RankId=B.RankId" +
                        " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                        " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                        " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
                        " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                        " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
                        " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
                        " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId) and fwd.StepId=@stepcount " +
                        " inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId " +
                        " where trnicrd.Status=0";
            }
            else if(stepcount == 11)
            {
                query = " SELECT distinct munit.UnitName,B.UnitId,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,fwd.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId,ran.RankAbbreviation RankName" +
                        " FROM BasicDetails B" +
                        " inner join MRank ran on ran.RankId=B.RankId " +
                        " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                        " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                        " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
                        " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                        " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
                        " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
                        " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId) and fwd.FwdStatusId=4 and trnicrd.Status=0" +
                        " inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId ";
            }
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var BasicDetailList = await connection.QueryAsync<BasicDetailVM>(query, new { UserId, stepcount, TypeId, applyForId });
                    int sno = 1;
                    var allrecord = (from e in BasicDetailList
                                     select new BasicDetailVM()
                                     {
                                         BasicDetailId = e.BasicDetailId,
                                         EncryptedId = protector.Protect(e.BasicDetailId.ToString()),
                                         Sno = sno++,
                                         Name = e.Name,
                                         ServiceNo = e.ServiceNo,
                                         DOB = e.DOB,
                                         DateOfCommissioning = e.DateOfCommissioning,
                                         PermanentAddress = e.PermanentAddress,
                                         IsTrnFwdId = e.IsTrnFwdId,
                                         StepCounter = e.StepCounter,
                                         StepId = e.StepId,
                                         ICardType = e.ICardType,
                                         ApplyFor = e.ApplyFor,
                                         ApplyForId = e.ApplyForId,
                                         RequestId = e.RequestId,
                                         IsFwdStatusId = e.IsFwdStatusId,
                                         TrackingId = e.TrackingId,
                                         RankName = e.RankName,
                                         UnitId = e.UnitId,
                                         UnitName= e.UnitName,
                                     }).ToList();
                    return await Task.FromResult(allrecord);

                }
            }
            catch(Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetALLBasicDetail");
                return null;
            }



        }
         
        public async Task<BasicDetailCrtAndUpdVM> GetByBasicDetailsId(int BasicDetailId)
        {
            //DTOBasicDetailRequest dd = new DTOBasicDetailRequest();
            //dd.MRank.RankAbbreviation = "";
            //dd.MArmedType.Abbreviation
            //string query = "select bas.*,ran.RankAbbreviation RankName,arm.Abbreviation ArmedType from BasicDetails bas " +
            //    " inner join MRank ran on ran.RankId=bas.RankId"+
            //    " inner join MArmedType arm on arm.ArmedId=bas.ArmedId"+
            //    " where bas.BasicDetailId=@BasicDetailId";

            string query = "select bas.*,"+
                            " issaut.Name IssuingAuthorityName,trnadd.State,trnadd.District,trnadd.PS,trnadd.PO,trnadd.Tehsil,trnadd.Village,trnadd.PinCode," +
                            " trnup.SignatureImagePath,trnup.PhotoImagePath,IdenMark1,IdenMark2,AadhaarNo,Height,bld.BloodGroup,bld.BloodGroupId," +
                            " regi.Abbreviation RegimentalName,Muni.UnitName,uni.UnitMapId UnitId,icardreq.TypeId,icardreq.RegistrationId," +
                            " ran.RankId,ran.RankAbbreviation RankName,arm.Abbreviation ArmedName,trnadd.AddressId,trnup.UploadId,trninfo.InfoId from BasicDetails bas" +
                            " inner join MIssuingAuthority issaut on issaut.IssuingAuthorityId=bas.IssuingAuthorityId" +
                            " inner join TrnAddress trnadd on trnadd.BasicDetailId=bas.BasicDetailId" +
                            " inner join TrnUpload trnup on trnup.BasicDetailId=bas.BasicDetailId"+
                            " inner join TrnIdentityInfo trninfo on trninfo.BasicDetailId=bas.BasicDetailId"+
                            " inner join MBloodGroup bld on bld.BloodGroupId=trninfo.BloodGroupId" +
                            " inner join MRank ran on ran.RankId=bas.RankId" +
                            " inner join MArmedType arm on arm.ArmedId=bas.ArmedId"+
                            " inner join MapUnit uni on uni.UnitMapId=bas.UnitId"+
                            " inner join MUnit Muni on Muni.UnitId=uni.UnitId"+
                            " left join TrnICardRequest icardreq on icardreq.BasicDetailId=bas.BasicDetailId and icardreq.Status in (0,1)" +
                            " left join MRegimental regi on regi.RegId=bas.RegimentalId" +
                            " where bas.BasicDetailId=@BasicDetailId";
            using (var connection = _contextDP.CreateConnection())
            {
                //data.MRank.RankAbbreviation
                //data.MArmedType.Abbreviation
                var BasicDetailList = await connection.QueryAsync<BasicDetailCrtAndUpdVM>(query, new { BasicDetailId });
                
                return BasicDetailList.SingleOrDefault();
            }
        }
        public async Task<BasicDetailCrtAndUpdVM> GetByRequestIdBesicDetails(int RequestId)
        {
            //DTOBasicDetailRequest dd = new DTOBasicDetailRequest();
            //dd.MRank.RankAbbreviation = "";
            //dd.MArmedType.Abbreviation
            //string query = "select bas.*,ran.RankAbbreviation RankName,arm.Abbreviation ArmedType from BasicDetails bas " +
            //    " inner join MRank ran on ran.RankId=bas.RankId"+
            //    " inner join MArmedType arm on arm.ArmedId=bas.ArmedId"+
            //    " where bas.BasicDetailId=@BasicDetailId";

            string query = "select bas.*," +
                            " issaut.Name IssuingAuthorityName,trnadd.State,trnadd.District,trnadd.PS,trnadd.PO,trnadd.Tehsil,trnadd.Village,trnadd.PinCode," +
                            " trnup.SignatureImagePath,trnup.PhotoImagePath,IdenMark1,IdenMark2,AadhaarNo,Height,bld.BloodGroup," +
                            " regi.Abbreviation RegimentalName,Muni.UnitName,uni.UnitMapId UnitId,icardreq.TypeId,icardreq.RegistrationId," +
                            " ran.RankId,ran.RankAbbreviation RankName,arm.Abbreviation ArmedName,trnadd.AddressId,trnup.UploadId,trninfo.InfoId from BasicDetails bas" +
                            " inner join MIssuingAuthority issaut on issaut.IssuingAuthorityId=bas.IssuingAuthorityId" +
                            " inner join TrnAddress trnadd on trnadd.BasicDetailId=bas.BasicDetailId" +
                            " inner join TrnUpload trnup on trnup.BasicDetailId=bas.BasicDetailId" +
                            " inner join TrnIdentityInfo trninfo on trninfo.BasicDetailId=bas.BasicDetailId" +
                            " inner join MBloodGroup bld on bld.BloodGroupId=trninfo.BloodGroupId" +
                            " inner join MRank ran on ran.RankId=bas.RankId" +
                            " inner join MArmedType arm on arm.ArmedId=bas.ArmedId" +
                            " inner join MapUnit uni on uni.UnitMapId=bas.UnitId" +
                            " inner join MUnit Muni on Muni.UnitId=uni.UnitId" +
                            " inner join TrnICardRequest icardreq on icardreq.BasicDetailId=bas.BasicDetailId and icardreq.Status=0 " +
                            " left join MRegimental regi on regi.RegId=bas.RegimentalId" +
                            " where icardreq.RequestId=@RequestId";
            using (var connection = _contextDP.CreateConnection())
            {
                //data.MRank.RankAbbreviation
                //data.MArmedType.Abbreviation
                var BasicDetailList = await connection.QueryAsync<BasicDetailCrtAndUpdVM>(query, new { RequestId });

                return BasicDetailList.SingleOrDefault();
            }
        }
        public async Task<List<DTODataExportsResponse>> GetBesicdetailsByRequestId(DTODataExportRequest Data)
        {
            
            string query = "update TrnFwds set IsComplete=1 where RequestId in @Ids update TrnStepCounter set StepId=5 where RequestId in @Ids  update TrnICardRequest set Status=1 where  RequestId in @Ids select bas.*," +
                            " trnadd.State,trnadd.District,trnadd.PS,trnadd.PO,trnadd.Tehsil,trnadd.Village,trnadd.PinCode," +
                            " trnup.SignatureImagePath,trnup.PhotoImagePath,IdenMark1,IdenMark2,AadhaarNo,Height,bld.BloodGroup," +
                            " regi.Abbreviation RegimentalName,Muni.UnitName,uni.UnitMapId UnitId,icardreq.TypeId,icardreq.RegistrationId," +
                            " ran.RankId,ran.RankAbbreviation RankName,arm.Abbreviation ArmedName,trnadd.AddressId,trnup.UploadId,trninfo.InfoId,MICardType.Name ICardType from BasicDetails bas" +
                            " inner join TrnAddress trnadd on trnadd.BasicDetailId=bas.BasicDetailId" +
                            " inner join TrnUpload trnup on trnup.BasicDetailId=bas.BasicDetailId" +
                            " inner join TrnIdentityInfo trninfo on trninfo.BasicDetailId=bas.BasicDetailId" +
                            " inner join MBloodGroup bld on bld.BloodGroupId=trninfo.BloodGroupId"+
                            " inner join MRank ran on ran.RankId=bas.RankId" +
                            " inner join MArmedType arm on arm.ArmedId=bas.ArmedId" +
                            " inner join MapUnit uni on uni.UnitMapId=bas.UnitId" +
                            " inner join MUnit Muni on Muni.UnitId=uni.UnitId" +
                            " inner join TrnICardRequest icardreq on icardreq.BasicDetailId=bas.BasicDetailId " + //and icardreq.Status=0 
                            " inner join MICardType MICardType on MICardType.TypeId=icardreq.TypeId " +
                            " left join MRegimental regi on regi.RegId=bas.RegimentalId" +
                            " where icardreq.RequestId in @Ids";
            int[] Ids = Data.Ids;
            using (var connection = _contextDP.CreateConnection())
            {
                //data.MRank.RankAbbreviation
                //data.MArmedType.Abbreviation
                var BasicDetailList = await connection.QueryAsync<DTODataExportsResponse>(query, new { Ids });

                return BasicDetailList.ToList();
            }
        }
        public async Task<DTOXMLDigitalResponse> GetDataDigitalXmlSign(DTODataExportRequest Data)
        {
            DTOXMLDigitalSignResponse dTOXMLDigitalSignResponse = new DTOXMLDigitalSignResponse();
            string query = "select bas.*, trnadd.State,trnadd.District,trnadd.PS,trnadd.PO,trnadd.Tehsil,trnadd.Village,trnadd.PinCode, "+
                           " trnup.SignatureImagePath,trnup.PhotoImagePath,IdenMark1,IdenMark2,AadhaarNo,Height,bld.BloodGroup, "+
                           " regi.Abbreviation RegimentalName,Muni.UnitName,uni.UnitMapId UnitId,icardreq.TypeId,icardreq.RegistrationId,"+ 
                           " ran.RankId,ran.RankAbbreviation RankName,arm.Abbreviation ArmedName,trnadd.AddressId,trnup.UploadId,"+
                           " trninfo.InfoId,MICardType.Name ICardType ,GETDATE() XmlCreatedOn," +
                           " App.Name ProApplyFor,reg.Name ProRegistraion,(select Name from MICardType where TypeId=icardreq.TypeId) ProType,users.DomainId ProDomainId,unit.UnitName ProUnitName,unit.Suffix ProSuffix,unit.Sus_no ProSUSNO,pro.Name ProName,ranks.RankAbbreviation ProRankName,pro.ArmyNo ProArmyName"+
                           " from BasicDetails bas "+
                           " inner join TrnAddress trnadd on trnadd.BasicDetailId=bas.BasicDetailId "+
                           " inner join TrnUpload trnup on trnup.BasicDetailId=bas.BasicDetailId "+
                           " inner join TrnIdentityInfo trninfo on trninfo.BasicDetailId=bas.BasicDetailId "+
                           " inner join MBloodGroup bld on bld.BloodGroupId=trninfo.BloodGroupId "+
                           " inner join MRank ran on ran.RankId=bas.RankId "+
                           " inner join MArmedType arm on arm.ArmedId=bas.ArmedId "+
                           " inner join MapUnit uni on uni.UnitMapId=bas.UnitId "+
                           " inner join MUnit Muni on Muni.UnitId=uni.UnitId "+
                           " inner join TrnICardRequest icardreq on icardreq.BasicDetailId=bas.BasicDetailId and icardreq.Status=0  "+
                           " inner join MICardType MICardType on MICardType.TypeId=icardreq.TypeId  "+
                           " inner join TrnDomainMapping trn on trn.Id=icardreq.TrnDomainMappingId"+
                           " inner join AspNetUsers users on users.Id = trn.AspNetUsersId "+
                           " inner join MapUnit mapuni on mapuni.UnitMapId = trn.UnitId "+
                           " inner join MUnit unit on unit.UnitId = mapuni.UnitId "+
                           " left join UserProfile pro on pro.UserId = trn.UserId "+
                           " inner join MRank ranks on ranks.RankId = pro.RankId"+
                           " inner join MApplyFor App on App.ApplyForId=bas.ApplyForId"+
                           " inner join MRegistration reg on App.ApplyForId=reg.ApplyForId and App.ApplyForId=bas.ApplyForId and reg.RegistrationId= icardreq.RegistrationId"+
                           " left join MRegimental regi on regi.RegId=bas.RegimentalId where icardreq.RequestId in @Ids";
            int[] Ids = Data.Ids;
            using (var connection = _contextDP.CreateConnection())
            {
                //data.MRank.RankAbbreviation
                //data.MArmedType.Abbreviation
                var BasicDetailList = await connection.QueryFirstAsync<dynamic>(query, new { Ids });
                if(BasicDetailList!=null)
                {
                    ApplicationDetails applicationDetails = new ApplicationDetails();
                   
                    applicationDetails.Name = BasicDetailList.Name;
                   
                   
                    applicationDetails.ServiceNo = BasicDetailList.ServiceNo;
                    applicationDetails.DOB = BasicDetailList.DOB;
                    applicationDetails.PlaceOfIssue = BasicDetailList.PlaceOfIssue;
                    applicationDetails.DateOfIssue = BasicDetailList.DateOfIssue;
                    applicationDetails.IssuingAuth = BasicDetailList.IssuingAuth;
                    applicationDetails.DateOfCommissioning = BasicDetailList.DateOfCommissioning;
                    applicationDetails.PaperIcardNo = BasicDetailList.PaperIcardNo;
                    applicationDetails.State = BasicDetailList.State;
                    applicationDetails.District = BasicDetailList.District;
                    applicationDetails.PS = BasicDetailList.PS;
                    applicationDetails.PO = BasicDetailList.PO;
                    applicationDetails.Tehsil = BasicDetailList.Tehsil;
                    applicationDetails.Village = BasicDetailList.Village;
                    applicationDetails.PinCode = BasicDetailList.PinCode;
                    applicationDetails.SignatureImagePath = BasicDetailList.SignatureImagePath;
                    applicationDetails.PhotoImagePath = BasicDetailList.PhotoImagePath;
                    applicationDetails.IdenMark1 = BasicDetailList.IdenMark1;
                    applicationDetails.IdenMark2 = BasicDetailList.IdenMark2;
                    applicationDetails.AadhaarNo = Convert.ToString(BasicDetailList.AadhaarNo);
                    applicationDetails.Height = Convert.ToString(BasicDetailList.Height);
                    applicationDetails.BloodGroup = BasicDetailList.BloodGroup;
                    applicationDetails.RegimentalName = BasicDetailList.RegimentalName;
                    applicationDetails.UnitName = BasicDetailList.UnitName;
                    applicationDetails.RankName = BasicDetailList.RankName;
                    applicationDetails.ArmedName = BasicDetailList.ArmedName;

                    applicationDetails.ICardType = BasicDetailList.ICardType;
                    applicationDetails.XmlCreatedOn = BasicDetailList.XmlCreatedOn;

                    Profiledtls profiledtls = new Profiledtls();
                    profiledtls.ProApplyFor = BasicDetailList.ProApplyFor;
                    profiledtls.ProRegistraion = BasicDetailList.ProRegistraion;
                    profiledtls.ProType = BasicDetailList.ProType;
                    profiledtls.ProDomainId = BasicDetailList.ProDomainId;
                    profiledtls.ProUnitName = BasicDetailList.ProUnitName;
                    profiledtls.ProSuffix = BasicDetailList.ProSuffix;
                    profiledtls.ProSUSNO = BasicDetailList.ProSUSNO;
                    profiledtls.ProName = BasicDetailList.ProName;
                    profiledtls.ProRankName = BasicDetailList.ProRankName;
                    profiledtls.ProArmyName = BasicDetailList.ProArmyName;

                    dTOXMLDigitalSignResponse.applicationDetails = applicationDetails;
                    dTOXMLDigitalSignResponse.profiledtls = profiledtls;
                }
                DTOXMLDigitalResponse dTOXMLDigitalResponse = new DTOXMLDigitalResponse();
                dTOXMLDigitalResponse.Header = dTOXMLDigitalSignResponse;
                return dTOXMLDigitalResponse;
            }
        }
        public async Task<List<ICardHistoryResponse>> ICardHistory(int RequestId)
        {
            string query = "select usersfrom.UserName FromDomain,profrom.Name FromProfile,ranlfrom.RankAbbreviation FromRank, " +
            " usersto.UserName ToDomain,proto.Name ToProfile,ranlto.RankAbbreviation ToRank ,"+
            " CASE fwd.FwdStatusId WHEN 1 THEN 'Pending' WHEN 2 THEN 'Approved' WHEN 3 THEN 'Reject' WHEN 4 THEN 'Internal Forward' END Status," +
            " fwd.UpdatedOn,isnull(fwd.Remark,'Nill') Remark, " +
            " fwd.IsComplete,(select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(fwd.RemarksIds,','))) Remarks2, " +
            " reason.Reason,postind.Authority,initres.UnitName " +
            " from TrnFwds fwd " +
            " inner join TrnStepCounter step"+
            " on fwd.RequestId=step.RequestId"+
            " inner join TrnDomainMapping mapfrom on mapfrom.AspNetUsersId=fwd.FromAspNetUsersId"+
            " inner join AspNetUsers usersfrom on usersfrom.Id=mapfrom.AspNetUsersId"+
            " inner join TrnDomainMapping mapto on mapto.AspNetUsersId=fwd.ToAspNetUsersId"+
            " inner join AspNetUsers usersto on usersto.Id=mapto.AspNetUsersId"+
            " left join UserProfile profrom"+
            " on mapfrom.UserId=profrom.UserId"+
            " inner join MRank ranlfrom on ranlfrom.RankId=profrom.RankId"+
            " left join UserProfile proto"+
            " on mapto.UserId=proto.UserId"+
            " left join TrnPostingOut postind on postind.Id=fwd.PostingOutId" +
            " left join MPostingReason reason on reason.Id=postind.ReasonId" +
            " left join MapUnit Munitres on Munitres.UnitMapId=postind.ToUnitID" +
            " left join MUnit initres on initres.UnitId=Munitres.UnitId" +
            " inner join MRank ranlto on ranlto.RankId=proto.RankId where fwd.RequestId=@RequestId" +
            " order by fwd.TrnFwdId asc";
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var BasicDetailList = await connection.QueryAsync<ICardHistoryResponse>(query, new { RequestId });

                    return BasicDetailList.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->ICardHistory");
                return null;
            }

        }

        public async Task<DTOICardTaskCountResponse> GetTaskCountICardRequest(int UserId,int Type, int applyForId)
        {
            #region Old code write by Kapoor Sir
            //string query = "";
            //if (Type == 1)
            //{
            //    query = "select COUNT(CASE WHEN StepId = 2 then 1 ELSE NULL END) as _2ndLevelPending," +
            //           " COUNT(CASE WHEN StepId = 3 then 1 ELSE NULL END) as _2ndLevelApproved," +
            //           " COUNT(CASE WHEN StepId = 7 then 1 ELSE NULL END) as _2ndLevelReject," +
            //           " COUNT(CASE WHEN StepId = 3 then 1 ELSE NULL END) as _3rdLevelPending," +
            //           " COUNT(CASE WHEN StepId = 4 then 1 ELSE NULL END) as _3rdLevelApproved," +
            //           " COUNT(CASE WHEN StepId = 8 then 1 ELSE NULL END) as _3rdLevelReject," +
            //           " COUNT(CASE WHEN StepId = 4 then 1 ELSE NULL END) as _4thLevelPending," +
            //           " COUNT(CASE WHEN StepId = 5 then 1 ELSE NULL END) as  _4thLevelApproved," +
            //           " COUNT(CASE WHEN StepId = 9 then 1 ELSE NULL END) as  _4thLevelReject," +
            //           " COUNT(CASE WHEN StepId = 5 then 1 ELSE NULL END) as  ExportPending," +
            //           " COUNT(CASE WHEN StepId = 6 then 1 ELSE NULL END) as  ExportApproved," +
            //           " COUNT(CASE WHEN StepId = 10 then 1 ELSE NULL END) as ExportReject" +
            //           " from TrnStepCounter cou" +
            //           " inner join TrnICardRequest req on cou.RequestId=req.RequestId and cou.ApplyForId=@applyForId" +
            //           " inner join TrnDomainMapping trndo on trndo.Id=req.TrnDomainMappingId" +
            //           " where AspNetUsersId=@UserId";
            //}
            //else if (Type == 2) 
            //{
            //    query = "declare @_2ndLevelPending int declare @_2ndLevelApproved int declare @_2ndLevelReject int" +
            //            " declare @_3rdLevelPending int declare @_3rdLevelApproved int declare @_3rdLevelReject int" +
            //            " declare @_4thLevelPending int declare @_4thLevelApproved int declare @_4thLevelReject int" +
            //            " declare @ExportPending int declare @ExportApproved int declare @ExportReject int" +
            //            " select @_2ndLevelPending=COUNT(distinct fwd.RequestId) from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId where ToAspNetUsersId=@UserId and IsComplete=0 and TypeId=2" +
            //            " select @_2ndLevelApproved=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId where FromAspNetUsersId=@UserId and TypeId=3" +
            //            " select @_2ndLevelReject=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId where FromAspNetUsersId=@UserId and fwd.StepId=7 and TypeId=1" +
            //            " select @_3rdLevelPending=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId where ToAspNetUsersId=@UserId and IsComplete=0 and TypeId=3" +
            //            " select @_3rdLevelApproved=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId where FromAspNetUsersId=@UserId and TypeId=4" +
            //            " select @_3rdLevelReject=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId where FromAspNetUsersId=@UserId and fwd.StepId=8 and TypeId=1" +
            //            " select @_4thLevelPending=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId where ToAspNetUsersId=@UserId and IsComplete=0 and cou.StepId=4" +
            //            " select @_4thLevelApproved=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId inner join TrnICardRequest trncard  on trncard.RequestId=cou.RequestId  where ToAspNetUsersId=@UserId and  trncard.Status=1" +
            //            " select @_4thLevelReject=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId where FromAspNetUsersId=@UserId and fwd.StepId=9 and TypeId=1" +
            //            " select @ExportPending=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId inner join TrnICardRequest trncard  on trncard.RequestId=cou.RequestId where ToAspNetUsersId=@UserId and IsComplete=0 and trncard.Status=0" +
            //            " select @ExportApproved=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId inner join TrnICardRequest trncard  on trncard.RequestId=cou.RequestId  where ToAspNetUsersId=@UserId and  trncard.Status=1" +
            //            " select @ExportReject=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId where FromAspNetUsersId=@UserId and fwd.StepId=10 and TypeId=1" +
            //            " select @_2ndLevelPending _2ndLevelPending,@_2ndLevelApproved _2ndLevelApproved,@_2ndLevelReject _2ndLevelReject,@_3rdLevelPending _3rdLevelPending,@_3rdLevelApproved _3rdLevelApproved,@_3rdLevelReject _3rdLevelReject, @_4thLevelPending _4thLevelPending,@_4thLevelApproved _4thLevelApproved,@_4thLevelReject _4thLevelReject,@ExportPending ExportPending,@ExportApproved ExportApproved,@ExportReject ExportReject";

            //}

            //using (var connection = _contextDP.CreateConnection())
            //{
            //    try
            //    {
            //        var ret = await connection.QueryAsync<DTOICardTaskCountResponse>(query, new { UserId, applyForId });
            //        return ret.SingleOrDefault();
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogError(1001, ex, "BasicDetailDB->GetTaskCountICardRequest");
            //        return null;
            //    }

            //}
            #endregion end code write by Kapoor Sir

            string query = "";
            if (Type==1) // Submitted
            {
                query = "declare @ToDrafted int=0 declare @ToSubmitted int=0 declare @ToCompleted int=0 declare @ToRejected int=0" +
                        " select @ToDrafted=COUNT(distinct req.RequestId) from TrnDomainMapping domain" +
                        " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                        " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId where domain.AspNetUsersId=@UserId and trnstepcout.StepId=1 and trnstepcout.ApplyForId=@applyForId " +

                        " select @ToSubmitted=COUNT(distinct req.RequestId) from TrnDomainMapping domain" +
                        " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                        " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId where domain.AspNetUsersId=@UserId and trnstepcout.StepId>1 and trnstepcout.ApplyForId=@applyForId " +

                        " select @ToCompleted=COUNT(distinct req.RequestId) from TrnDomainMapping domain" +
                        " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                        " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId where domain.AspNetUsersId=@UserId and req.Status=1 and trnstepcout.ApplyForId=@applyForId " +

                        " select @ToRejected=COUNT(distinct fwd.RequestId) from TrnDomainMapping domain" +
                        " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                        " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId" +
                        " inner join TrnFwds fwd on fwd.RequestId= trnstepcout.RequestId where fwd.ToAspNetUsersId=@UserId and req.Status=0 and req.StepId in(7,8,9,10) and trnstepcout.ApplyForId=@applyForId " +

                        " select @ToDrafted ToDrafted,@ToSubmitted ToSubmitted,@ToCompleted ToCompleted,@ToRejected ToRejected";
            }
            else if (Type == 2) // Pending
            {
                query = " declare @_2ndLevelPending int declare @_2ndLevelApproved int declare @_2ndLevelReject int" +
                        " declare @_3rdLevelPending int declare @_3rdLevelApproved int declare @_3rdLevelReject int" +
                        " declare @_4thLevelPending int declare @_4thLevelApproved int declare @_4thLevelReject int" +
                        " declare @ExportPending int declare @ExportApproved int declare @ExportReject int declare @ToInternalForward int" +
                        " select @_2ndLevelPending=COUNT(distinct fwd.RequestId) from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId where ToAspNetUsersId=@UserId and IsComplete=0 and TypeId=2" +
                        " select @_2ndLevelApproved=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId where FromAspNetUsersId=@UserId and TypeId=3" +
                        " select @_2ndLevelReject=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId where FromAspNetUsersId=@UserId and fwd.StepId=7 and TypeId=1" +
                        " select @_3rdLevelPending=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId where ToAspNetUsersId=@UserId and IsComplete=0 and TypeId=3" +
                        " select @_3rdLevelApproved=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId where FromAspNetUsersId=@UserId and TypeId=4" +
                        " select @_3rdLevelReject=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId where FromAspNetUsersId=@UserId and fwd.StepId=8 and TypeId=1" +
                        " select @_4thLevelPending=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId where ToAspNetUsersId=@UserId and IsComplete=0 and cou.StepId=4" +
                        " select @_4thLevelApproved=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId inner join TrnICardRequest trncard  on trncard.RequestId=cou.RequestId  where ToAspNetUsersId=@UserId and  trncard.Status=1" +
                        " select @_4thLevelReject=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId where FromAspNetUsersId=@UserId and fwd.StepId=9 and TypeId=1" +
                        " select @ExportPending=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId inner join TrnICardRequest trncard  on trncard.RequestId=cou.RequestId where ToAspNetUsersId=@UserId and IsComplete=0 and trncard.Status=0" +
                        " select @ExportApproved=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId inner join TrnICardRequest trncard  on trncard.RequestId=cou.RequestId  where ToAspNetUsersId=@UserId and  trncard.Status=1" +
                        " select @ExportReject=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId where FromAspNetUsersId=@UserId and fwd.StepId=10 and TypeId=1" +
                        " select @ToInternalForward=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId inner join TrnICardRequest trncard  on trncard.RequestId=cou.RequestId where FromAspNetUsersId=@UserId and FwdStatusId=4 and trncard.Status=0" +
                        " select @_2ndLevelPending _2ndLevelPending,@_2ndLevelApproved _2ndLevelApproved,@_2ndLevelReject _2ndLevelReject,@_3rdLevelPending _3rdLevelPending,@_3rdLevelApproved _3rdLevelApproved,@_3rdLevelReject _3rdLevelReject, @_4thLevelPending _4thLevelPending,@_4thLevelApproved _4thLevelApproved,@_4thLevelReject _4thLevelReject,@ExportPending ExportPending,@ExportApproved ExportApproved,@ExportReject ExportReject,@ToInternalForward ToInternalForward";
                        
            } 
          
            using (var connection = _contextDP.CreateConnection())
            {
                try
                {
                    var ret = await connection.QueryAsync<DTOICardTaskCountResponse>(query, new { UserId, applyForId });
                    return ret.SingleOrDefault();
                }
                catch(Exception ex)
                {
                    _logger.LogError(1001, ex, "BasicDetailDB->GetTaskCountICardRequest");
                    return null;
                }

            }
        }

        public async Task<List<DTONotificationResponse>> GetNotification(int UserId, int Type, int applyForId)
        {
            string query = "select dis.DisplayId,Spanname,Message,ranks.RankAbbreviation,bas.Name,bas.ServiceNo,tre.TrackingId,uplod.PhotoImagePath,dis.Url  from TrnNotification noti" +
                            " inner join TrnNotificationDisplay dis on noti.DisplayId=dis.DisplayId"+
                            " inner join AspNetUsers users on users.Id=noti.SentAspNetUsersId"+
                            " inner join TrnStepCounter stepc on stepc.RequestId=noti.RequestId "+
                            " inner join TrnICardRequest tre on tre.RequestId = noti.RequestId " +
                             " inner join BasicDetails bas on bas.BasicDetailId=tre.BasicDetailId" +
                            " inner join MRank ranks on ranks.RankId=bas.RankId" +
                            " inner join TrnUpload uplod on uplod.BasicDetailId=bas.BasicDetailId" +
                            " where noti.ReciverAspNetUsersId=@UserId and NotificationTypeId=@Type and stepc.applyforId=@applyForId and [Read]=0 and ReciverAspNetUsersId!=SentAspNetUsersId";
        
            using (var connection = _contextDP.CreateConnection())
            {
                //data.MRank.RankAbbreviation
                //data.MArmedType.Abbreviation
                var ret = await connection.QueryAsync<DTONotificationResponse>(query, new { UserId, Type, applyForId });


                // var allProducts = ret.Concat(ret1) .ToList();


                return ret.ToList();
            }
        }
        public async Task<List<DTONotificationResponse>> GetNotificationRequestId(int UserId, int Type, int applyForId)
        {

            string query = "select Distinct tre.RequestId, dis.DisplayId,Spanname + 'self' Spanname,Message,ranks.RankAbbreviation,bas.Name,bas.ServiceNo,tre.TrackingId,uplod.PhotoImagePath,CASE WHEN dis.DisplayId in (7,8,9,10,17,18,19,20) THEN dis.Url ELSE '' END AS Url  from TrnNotification noti " +
                            " inner join TrnNotificationDisplay dis on noti.DisplayId = dis.DisplayId" +
                            " inner join AspNetUsers users on users.Id = noti.SentAspNetUsersId" +
                            " inner join TrnICardRequest tre on tre.RequestId = noti.RequestId" +
                            " inner join TrnDomainMapping dmap on dmap.Id = tre.TrnDomainMappingId" +
                            " inner join TrnStepCounter cou on cou.RequestId=tre.RequestId" +
                            " inner join BasicDetails bas on bas.BasicDetailId=tre.BasicDetailId" +
                            " inner join MRank ranks on ranks.RankId=bas.RankId"+
                             " inner join TrnUpload uplod on uplod.BasicDetailId=bas.BasicDetailId" +
                            " where NotificationTypeId = @Type and dmap.AspNetUsersId = @UserId and [Read]=0 and cou.applyforId=@applyForId and ReciverAspNetUsersId=SentAspNetUsersId";

            using (var connection = _contextDP.CreateConnection())
            {
                //data.MRank.RankAbbreviation
                //data.MArmedType.Abbreviation
                var ret = await connection.QueryAsync<DTONotificationResponse>(query, new { UserId, Type, applyForId });
               

              
                return ret.ToList();
            }
        }
        public async Task<List<MRecordOffice>> GetROListByArmedId(byte ArmedId)
        {
            return await _context.MRecordOffice.Where(x => x.ArmedId == ArmedId).ToListAsync();
        }
        public async Task<IEnumerable<SelectListItem>> GetRODDLIdSelected(byte ArmedId)
        {
            try
            {
                var ROOptions = await _context.MRecordOffice.Where(x => x.ArmedId == ArmedId)
                    .Select(a =>
                      new SelectListItem
                      {
                          Value = a.RecordOfficeId.ToString(),
                          Text = a.Name
                      }).ToListAsync();

                var ddfirst = new SelectListItem()
                {
                    Value = null,
                    Text = "Please Select"
                };
                ROOptions.Insert(0, ddfirst);
                return new SelectList(ROOptions, "Value", "Text");
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetRODDLIdSelected");
                return null;
            }

        }

        public async Task<DTOApplicationTrack> ApplicationHistory(string TrackingId)
        {
            DTOApplicationTrack lst=new DTOApplicationTrack();
                    
            string query = " select ran.RankAbbreviation RankName,bas.Name,bas.ServiceNo ArmyNo,unit.UnitName,uplod.PhotoImagePath," +
                           " ranfrom.RankAbbreviation FromRank,pr.Name FromName,pr.ArmyNo FromArmyNo,users.DomainId" +
                           " from BasicDetails bas " +
                           " inner join TrnICardRequest req on bas.BasicDetailId=req.BasicDetailId" +
                           " inner join TrnUpload uplod on bas.BasicDetailId=uplod.BasicDetailId" +
                           " inner join MRank ran on bas.RankId=ran.RankId" +
                           " inner join MapUnit muni on bas.UnitId=muni.UnitMapId" +
                           " inner join MUnit unit on  muni.UnitId=unit.UnitId" +
                           " inner join TrnDomainMapping map on map.Id= req.TrnDomainMappingId" +
                           " inner join AspNetUsers users on map.AspNetUsersId=users.Id" +
                           " inner join UserProfile pr on pr.UserId = map.UserId" +
                           " inner join MRank ranfrom on pr.RankId=ranfrom.RankId" +
                           " where req.Status=0 and req.TrackingId=@TrackingId";

                           //" select fwd.FwdStatusId,fwd.stepId,fwd.UpdatedOn,step.Name,fwd.IsComplete" +
                           //" from TrnFwds fwd " +
                           //" inner join TrnICardRequest req on fwd.RequestId=req.RequestId" +
                           //" inner join MStepCounterStep step on fwd.StepId=step.StepId" +
                           //"  where fwd.RequestId=@RequestId" +
                           //" order by fwd.TrnFwdId asc";
            using (var connection = _contextDP.CreateConnection())
            {
                //data.MRank.RankAbbreviation
                //data.MArmedType.Abbreviation
                var ret = await connection.QueryAsync<DTOApplicationDetails>(query, new { TrackingId });


                // var allProducts = ret.Concat(ret1) .ToList();


                lst.dTOApplicationDetails = ret.FirstOrDefault();
            }
            query = " select fwd.FwdStatusId,fwd.stepId,fwd.UpdatedOn,step.Name,fwd.IsComplete," +
                " isnull(fwd.Remark,'') Remark," +
                " (select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(fwd.RemarksIds,','))) Remark2" +
            " from TrnFwds fwd " +
            " inner join TrnICardRequest req on fwd.RequestId=req.RequestId" +
            " inner join MStepCounterStep step on fwd.StepId=step.StepId" +
            "  where req.Status=0 and req.TrackingId=@TrackingId" +
            " order by fwd.TrnFwdId asc";
            using (var connection = _contextDP.CreateConnection())
            {
                //data.MRank.RankAbbreviation
                //data.MArmedType.Abbreviation
                var ret1 = await connection.QueryAsync<DTOTrackHistory>(query, new { TrackingId });


                // var allProducts = ret.Concat(ret1) .ToList();


                lst.dTOTrackHistory = ret1.ToList();
            }

            return lst; 
        }
    }
}
