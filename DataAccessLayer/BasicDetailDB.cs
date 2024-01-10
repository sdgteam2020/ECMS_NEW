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
using System.Data.Entity;
using static Dapper.SqlMapper;

namespace DataAccessLayer
{
    public class BasicDetailDB : GenericRepositoryDL<BasicDetail>, IBasicDetailDB
    {
        protected readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly IDataProtector protector;
        public BasicDetailDB(ApplicationDbContext context, DapperContext contextDP, IDataProtectionProvider dataProtectionProvider, DataProtectionPurposeStrings dataProtectionPurposeStrings) : base(context)
        {
            _context = context;
            _contextDP=contextDP;
            // Pass the purpose string as a parameter
            this.protector = dataProtectionProvider.CreateProtector(
                dataProtectionPurposeStrings.AFSACIdRouteValue);
        }
        public async Task<bool> SaveBasicDetailsWithAll(BasicDetail Data, MTrnAddress address, MTrnUpload trnUpload, MTrnIdentityInfo mTrnIdentityInfo, MTrnICardRequest mTrnICardRequest, MStepCounter mStepCounter)
        {

            try
            {
                

                if (Data.BasicDetailId == 0)
                {
                    using var transaction = _context.Database.BeginTransaction();
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
                }
                else
                {
                    using var transaction = _context.Database.BeginTransaction();
                    address.BasicDetailId = Data.BasicDetailId;
                    trnUpload.BasicDetailId = Data.BasicDetailId;
                    mTrnIdentityInfo.BasicDetailId = Data.BasicDetailId;

                    //_context.Entry(Data).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    //_context.Update(Data);
                    //_context.SaveChangesAsync();

                    _context.Update(address);
                    await _context.SaveChangesAsync();
                    _context.Update(trnUpload);
                    await _context.SaveChangesAsync();
                    _context.Update(mTrnIdentityInfo);
                    await _context.SaveChangesAsync();
                    transaction.Commit();

                }
                //do other things, then commit or rollback
               
                return true;
            }
           catch (Exception ex) { return false; }
            

            
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
            //var BasicDetailList = _context.BasicDetails.Where(x => x.IsDeleted == false && x.Updatedby == UserId).ToList();
            int? applyfor = 0;
            if (apply == 0) applyfor = null; else applyfor = apply;

            string query = "";

            if (stepcount == 0 || stepcount == 1)//////For Fwd Record
            {
                query = "SELECT B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.Name ICardType,trnicrd.RequestId,fwd.Remark,isnull(fwd.Status,1) Reject,Afor.Name ApplyFor,Afor.ApplyForId ,trnicrd.TrackingId FROM BasicDetails B " +
                        "inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId " +
                        "inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                        "inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId " +
                        "inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
                        "inner join UserProfile pr on pr.UserId = trnicrd.Updatedby " +
                        "inner join TrnDomainMapping map on map.UserId=pr.UserId " +
                        "left join TrnFwds fwd on fwd.ToAspNetUsersId= map.AspNetUsersId and fwd.IsComplete=0 and fwd.RequestId=trnicrd.RequestId " +
                        "WHERE map.AspNetUsersId = @UserId and Afor.ApplyForId=ISNULL(@applyfor,Afor.ApplyForId) and trnicrd.Status=0 ORDER BY B.UpdatedOn DESC";

            }
            else if (stepcount == 2 || stepcount == 3 || stepcount == 4 || stepcount == 5 || stepcount == 6)//IO
            {
               if(TypeId==2)
                {
                    query = " SELECT distinct B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,Afor.Name ApplyFor,Afor.ApplyForId ,trnicrd.TrackingId" +
            " FROM BasicDetails B" +
            " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
            " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
            " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
            " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
            " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and Afor.ApplyForId=ISNULL(@applyfor,Afor.ApplyForId) and C.StepId = @stepcount where trnicrd.Status=0";
                }
                else if (TypeId == 3)
                {
                    query = " SELECT distinct B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,Afor.Name ApplyFor,Afor.ApplyForId ,trnicrd.TrackingId" +
            " FROM BasicDetails B" +
            " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
            " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
            " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
            " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
            " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and Afor.ApplyForId=ISNULL(@applyfor,Afor.ApplyForId) and C.StepId = @stepcount where trnicrd.Status=0";
                }


            }
            else if (stepcount == 7 || stepcount == 8 || stepcount == 9 || stepcount == 10 )//Reject From IO
            {
               
                query = "SELECT distinct B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId,isnull(fwd.Status,1) Reject ,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId" +
                " FROM BasicDetails B" +
                " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
                " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
                " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
                " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.ToAspNetUsersId = @UserId and Afor.ApplyForId=ISNULL(@applyfor,Afor.ApplyForId) and fwd.Status=0 and C.StepId = @stepcount where trnicrd.Status=0";
            }
         
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
            using (var connection = _contextDP.CreateConnection())
            {
                var BasicDetailList = await connection.QueryAsync<BasicDetailVM>(query, new { UserId, stepcount, TypeId,applyfor });
                //List<MRegistration> RegistrationList = await _context.MRegistration.ToListAsync();
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
                                     StepCounter = e.StepCounter,
                                     StepId = e.StepId,
                                     ICardType = e.ICardType,
                                     //Registration = (from r in RegistrationList
                                     //                where r.RegistrationId == e.RegistrationId
                                     //                select r).First(),
                                     //RegistrationId = e.RegistrationId,
                                     ApplyFor=e.ApplyFor,
                                     ApplyForId=e.ApplyForId,
                                     RequestId = e.RequestId,
                                     Reject = e.Reject,
                                     Remark = e.Remark,
                                     TrackingId=e.TrackingId,
                                 }).ToList();
                return await Task.FromResult(allrecord);

            }

        }
        public async Task<List<BasicDetailVM>> GetALLBasicDetail(int UserId,int stepcount, int TypeId, int applyForId)
        {
            //var BasicDetailList = _context.BasicDetails.Where(x => x.IsDeleted == false && x.Updatedby == UserId).ToList();

            string query = "";

            if(stepcount == 0 || stepcount == 1)//////For Fwd Record
            {
                query = "SELECT B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.Name ICardType,trnicrd.RequestId,fwd.Remark,isnull(fwd.Status,1) Reject,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId  FROM BasicDetails B " +
                        "inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId " +
                        "inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                        "inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId " +
                        "inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
                        "inner join UserProfile pr on pr.UserId = trnicrd.Updatedby " +
                        "inner join TrnDomainMapping map on map.UserId=pr.UserId " +
                        "left join TrnFwds fwd on fwd.ToAspNetUsersId= map.AspNetUsersId and fwd.IsComplete=0 and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId) and fwd.RequestId=trnicrd.RequestId " +
                        "WHERE map.AspNetUsersId = @UserId and trnicrd.Status=0 ORDER BY B.UpdatedOn DESC";
                
            }
            else if (stepcount == 2 || stepcount == 3 || stepcount == 4 || stepcount == 5)//IO
            {
                if(TypeId==1)///For Icard Submit
                {
                    query = " SELECT distinct B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId" +
               " FROM BasicDetails B" +
               " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
               " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
               " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
               " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
               " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId) and fwd.TypeId=@stepcount and C.StepId = @stepcount and trnicrd.Status=0";
                }
                else if (TypeId == 2) //// For For Action
                {
                    query = " SELECT distinct B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId" +
               " FROM BasicDetails B" +
               " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
               " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
               " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
               " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
               " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.ToAspNetUsersId = @UserId and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId) and fwd.TypeId=@stepcount and C.StepId = @stepcount and trnicrd.Status=0";


                }
                else //if (TypeId == 3) //// For For Show
                {
                    TypeId = stepcount - 1;
                    //if (TypeId == 3) TypeId = 2;
                    //if (TypeId == 4) TypeId = 3;
                    //if (TypeId == 5) TypeId = 4;
                    query = " SELECT distinct B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,Afor.Name ApplyFor,Afor.ApplyForId" +
               " FROM BasicDetails B" +
               " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
               " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
               " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
               " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
               " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.ToAspNetUsersId = @UserId and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId) and fwd.TypeId=@TypeId and fwd.IsComplete=1 and trnicrd.Status=0";


                }
            }
            else if (stepcount == 7 || stepcount == 8 || stepcount == 9 || stepcount == 10)//Reject From IO
            {

                query = "SELECT distinct B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId,isnull(fwd.Status,1) Reject ,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId" +
                " FROM BasicDetails B" +
                " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
                " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
                " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
                " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId) and fwd.Status=0 where trnicrd.Status=0";
            }

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
            using (var connection = _contextDP.CreateConnection())
            {
                var BasicDetailList = await connection.QueryAsync<BasicDetailVM>(query, new { UserId, stepcount, TypeId, applyForId });
                //List<MRegistration> RegistrationList = await _context.MRegistration.ToListAsync();
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
                                     StepCounter = e.StepCounter,
                                     StepId = e.StepId,
                                     ICardType=e.ICardType,
                                     //Registration= (from r in RegistrationList
                                     //               where r.RegistrationId == e.RegistrationId 
                                     //               select r).First(),
                                     //RegistrationId = e.RegistrationId,
                                     ApplyFor = e.ApplyFor,
                                     ApplyForId = e.ApplyForId,
                                     RequestId =e.RequestId,
                                     Reject=e.Reject,
                                     TrackingId = e.TrackingId,
                                 }).ToList();
                return await Task.FromResult(allrecord);

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
                            " trnadd.State,trnadd.District,trnadd.PS,trnadd.PO,trnadd.Tehsil,trnadd.Village,trnadd.PinCode,"+
                            " trnup.SignatureImagePath,trnup.PhotoImagePath,IdenMark1,IdenMark2,AadhaarNo,Height,bld.BloodGroup," +
                            " regi.Abbreviation RegimentalName,Muni.UnitName,uni.UnitMapId UnitId,icardreq.TypeId,icardreq.RegistrationId," +
                            " ran.RankId,ran.RankAbbreviation RankName,arm.Abbreviation ArmedName,trnadd.AddressId,trnup.UploadId,trninfo.InfoId from BasicDetails bas" +
                            " inner join TrnAddress trnadd on trnadd.BasicDetailId=bas.BasicDetailId"+
                            " inner join TrnUpload trnup on trnup.BasicDetailId=bas.BasicDetailId"+
                            " inner join TrnIdentityInfo trninfo on trninfo.BasicDetailId=bas.BasicDetailId"+
                            " inner join MBloodGroup bld on bld.BloodGroupId=trninfo.BloodGroupId" +
                            " inner join MRank ran on ran.RankId=bas.RankId" +
                            " inner join MArmedType arm on arm.ArmedId=bas.ArmedId"+
                            " inner join MapUnit uni on uni.UnitMapId=bas.UnitId"+
                            " inner join MUnit Muni on Muni.UnitId=uni.UnitId"+
                            " inner join TrnICardRequest icardreq on icardreq.BasicDetailId=bas.BasicDetailId and icardreq.Status=0 "+
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
        public async Task<List<DTODataExportsResponse>> GetBesicdetailsByRequestId(DTODataExportRequest Data)
        {
            //DTOBasicDetailRequest dd = new DTOBasicDetailRequest();
            //dd.MRank.RankAbbreviation = "";
            //dd.MArmedType.Abbreviation
            //string query = "select bas.*,ran.RankAbbreviation RankName,arm.Abbreviation ArmedType from BasicDetails bas " +
            //    " inner join MRank ran on ran.RankId=bas.RankId"+
            //    " inner join MArmedType arm on arm.ArmedId=bas.ArmedId"+
            //    " where bas.BasicDetailId=@BasicDetailId";

            string query = "select bas.*," +
                            " trnadd.State,trnadd.District,trnadd.PS,trnadd.PO,trnadd.Tehsil,trnadd.Village,trnadd.PinCode," +
                            " trnup.SignatureImagePath,trnup.PhotoImagePath,IdenMark1,IdenMark2,AadhaarNo,Height,bld.BloodGroup," +
                            " regi.Abbreviation RegimentalName,Muni.UnitName,uni.UnitMapId UnitId,icardreq.TypeId,icardreq.RegistrationId," +
                            " ran.RankId,ran.RankAbbreviation RankName,arm.Abbreviation ArmedName,trnadd.AddressId,trnup.UploadId,trninfo.InfoId from BasicDetails bas" +
                            " inner join TrnAddress trnadd on trnadd.BasicDetailId=bas.BasicDetailId" +
                            " inner join TrnUpload trnup on trnup.BasicDetailId=bas.BasicDetailId" +
                            " inner join TrnIdentityInfo trninfo on trninfo.BasicDetailId=bas.BasicDetailId" +
                            " inner join MBloodGroup bld on bld.BloodGroupId=trninfo.BloodGroupId"+
                            " inner join MRank ran on ran.RankId=bas.RankId" +
                            " inner join MArmedType arm on arm.ArmedId=bas.ArmedId" +
                            " inner join MapUnit uni on uni.UnitMapId=bas.UnitId" +
                            " inner join MUnit Muni on Muni.UnitId=uni.UnitId" +
                            " inner join TrnICardRequest icardreq on icardreq.BasicDetailId=bas.BasicDetailId and icardreq.Status=0 " +
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
        public async Task<List<ICardHistoryResponse>> ICardHistory(int RequestId)
        {
            string query = "select usersfrom.UserName FromDomain,profrom.Name FromProfile,ranlfrom.RankAbbreviation FromRank, " +
            " usersto.UserName ToDomain,proto.Name ToProfile,ranlto.RankAbbreviation ToRank ,"+
            " CASE fwd.Status WHEN 1 THEN 'Approved' WHEN 0 THEN 'Reject'  END Status,fwd.UpdatedOn,isnull(fwd.Remark,'Nill') Remark,fwd.IsComplete,(select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(fwd.RemarksIds,','))) Remarks2 from TrnFwds fwd " +
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
            " inner join MRank ranlto on ranlto.RankId=proto.RankId where fwd.RequestId=@RequestId" +
            " order by fwd.TrnFwdId asc";
            using (var connection = _contextDP.CreateConnection())
            {
                //data.MRank.RankAbbreviation
                //data.MArmedType.Abbreviation
                var BasicDetailList = await connection.QueryAsync<ICardHistoryResponse>(query, new { RequestId });

                return BasicDetailList.ToList();
            }
        }

        public async Task<DTOICardTaskCountResponse> GetTaskCountICardRequest(int UserId,int Type, int applyForId)
        {
            string query = "";
            if (Type==1)
            {
                query = "select COUNT(CASE WHEN StepId = 2 then 1 ELSE NULL END) as IOPending," +
                       " COUNT(CASE WHEN StepId = 3 then 1 ELSE NULL END) as IOApproved," +
                       " COUNT(CASE WHEN StepId = 7 then 1 ELSE NULL END) as IOReject," +
                       " COUNT(CASE WHEN StepId = 3 then 1 ELSE NULL END) as GSOPending," +
                       " COUNT(CASE WHEN StepId = 4 then 1 ELSE NULL END) as GSOApproved," +
                       " COUNT(CASE WHEN StepId = 8 then 1 ELSE NULL END) as GSOReject," +
                       " COUNT(CASE WHEN StepId = 4 then 1 ELSE NULL END) as MIPending," +
                       " COUNT(CASE WHEN StepId = 5 then 1 ELSE NULL END) as  MIApproved," +
                       " COUNT(CASE WHEN StepId = 9 then 1 ELSE NULL END) as  MIReject," +
                       " COUNT(CASE WHEN StepId = 5 then 1 ELSE NULL END) as  HQPending," +
                       " COUNT(CASE WHEN StepId = 6 then 1 ELSE NULL END) as  HQApproved," +
                       " COUNT(CASE WHEN StepId = 10 then 1 ELSE NULL END) as HQReject" +
                       " from TrnStepCounter cou" +
                       " inner join TrnICardRequest req on cou.RequestId=req.RequestId and cou.ApplyForId=@applyForId" +
                       " inner join TrnDomainMapping trndo on trndo.Id=req.TrnDomainMappingId" + 
                       " where AspNetUsersId=@UserId";
            }
            else if (Type == 2)
            {
                query = "declare @IOPending int declare @IOApproved int declare @IOReject int"+
                        " declare @GSOPending int declare @GSOApproved int declare @GSOReject int"+
                        " declare @MIPending int declare @MIApproved int declare @MIReject int"+
                        " declare @HQPending int declare @HQApproved int declare @HQReject int"+
                        " select @IOPending=COUNT(distinct fwd.RequestId) from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId where ToAspNetUsersId=@UserId and IsComplete=0 and TypeId=2" +
                        " select @IOApproved=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId where FromAspNetUsersId=@UserId and TypeId=3" +
                        " select @IOReject=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId where FromAspNetUsersId=@UserId and Status=0 and TypeId=1" +
                        " select @GSOPending=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId where ToAspNetUsersId=@UserId and IsComplete=0 and TypeId=3" +
                        " select @GSOApproved=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId where FromAspNetUsersId=@UserId and TypeId=4" +
                        " select @GSOReject=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId where FromAspNetUsersId=@UserId and Status=0 and TypeId=1" +
                        " select @MIPending=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId where ToAspNetUsersId=@UserId and IsComplete=0 and TypeId=4" +
                        " select @MIApproved=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId where FromAspNetUsersId=@UserId and TypeId=5" +
                        " select @MIReject=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId where FromAspNetUsersId=@UserId and Status=0 and TypeId=1" +
                        " select @HQPending=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId where ToAspNetUsersId=@UserId and IsComplete=0 and TypeId=5" +
                        " select @HQApproved=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId where FromAspNetUsersId=@UserId and TypeId=6" +
                        " select @HQReject=COUNT(distinct fwd.RequestId)  from TrnFwds fwd inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId where FromAspNetUsersId=@UserId and Status=0 and TypeId=1" +
                        " select @IOPending IOPending,@IOApproved IOApproved,@IOReject IOReject,@GSOPending GSOPending,@GSOApproved GSOApproved,@GSOReject GSOReject, @MIPending MIPending,@MIApproved MIApproved,@MIReject MIReject,@HQPending HQPending,@HQApproved HQApproved,@HQReject HQReject";
                        
            }
          
            using (var connection = _contextDP.CreateConnection())
            {
                //data.MRank.RankAbbreviation
                //data.MArmedType.Abbreviation
                var ret = await connection.QueryAsync<DTOICardTaskCountResponse>(query, new { UserId, applyForId });

                return ret.SingleOrDefault();
            }
        }

        public async Task<List<DTONotificationResponse>> GetNotification(int UserId, int Type, int applyForId)
        {
            string query = "select dis.DisplayId,Spanname,Message from TrnNotification noti" +
                            " inner join TrnNotificationDisplay dis on noti.DisplayId=dis.DisplayId"+
                            " inner join AspNetUsers users on users.Id=noti.SentAspNetUsersId"+
                            " inner join TrnStepCounter stepc on stepc.RequestId=noti.RequestId "+
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

            string query = "select dis.DisplayId,Spanname + 'self' Spanname,Message from TrnNotification noti " +
                            " inner join TrnNotificationDisplay dis on noti.DisplayId = dis.DisplayId" +
                            " inner join AspNetUsers users on users.Id = noti.SentAspNetUsersId" +
                            " inner join TrnICardRequest tre on tre.RequestId = noti.RequestId" +
                            " inner join TrnDomainMapping dmap on dmap.Id = tre.TrnDomainMappingId" +
                            " inner join TrnStepCounter cou on cou.RequestId=tre.RequestId" +
                            " where NotificationTypeId = @Type and dmap.AspNetUsersId = @UserId and [Read]=0 and cou.applyforId=@applyForId and ReciverAspNetUsersId=SentAspNetUsersId";

            using (var connection = _contextDP.CreateConnection())
            {
                //data.MRank.RankAbbreviation
                //data.MArmedType.Abbreviation
                var ret = await connection.QueryAsync<DTONotificationResponse>(query, new { UserId, Type, applyForId });
               

              
                return ret.ToList();
            }
        }

       
    }
}
