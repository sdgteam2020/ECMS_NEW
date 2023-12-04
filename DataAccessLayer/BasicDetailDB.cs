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

namespace DataAccessLayer
{
    public class BasicDetailDB:GenericRepositoryDL<BasicDetail>,IBasicDetailDB
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
            this.protector = protector;
        }
        public async Task<List<DTOBasicDetailRequest>> GetALLBasicDetail(int UserId,int stepcount, int type)
        {
            //var BasicDetailList = _context.BasicDetails.Where(x => x.IsDeleted == false && x.Updatedby == UserId).ToList();

            string query = "";

            if(type==0)//////For Fwd Record
            {
                if(stepcount==1)
                {
                    query = "SELECT B.RegistrationType,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,B.PermanentAddress,C.StepId StepCounter,C.Id StepId,ty.TypeId ICardType,trnicrd.RequestId  FROM BasicDetails B " +
                         "inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId " +
                         "inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId " +
                         "inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
                         "inner join UserProfile pr on pr.UserId = trnicrd.Updatedby " +
                         "WHERE pr.Updatedby = @UserId ORDER BY B.UpdatedOn DESC";
                }
                else
                {
                    query = "SELECT B.RegistrationType,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,B.PermanentAddress,C.Step StepCounter,C.Id StepId,ty.TypeId ICardType,trnicrd.RequestId  FROM BasicDetails B " +
                          "inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId " +
                          "inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId " +
                          "inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
                          "WHERE trnicrd.Updatedby = @UserId and C.Step = @stepcount ORDER BY B.UpdatedOn DESC";
                }
               
            }
            else if (type == 1)//IO
            {
                query = "SELECT B.RegistrationType,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,B.PermanentAddress,C.Step StepCounter,C.Id StepId,ty.TypeId,trnicrd.RequestId  FROM BasicDetails B " +
                           "inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId " +
                           "inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId " +
                           "inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
                           "inner join UserProfile pro on pro.ArmyNo=B.ServiceNo " +
                           "inner join MMappingProfile mpro on mpro.UserId=pro.UserId " +
                           "WHERE  mpro.IOId = @UserId and C.Step = @stepcount ORDER BY B.UpdatedOn DESC";
            }
            else if (type == 2)////GSO
            {
                query = "SELECT B.RegistrationType,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,B.PermanentAddress,C.Step StepCounter,C.Id StepId,ty.TypeId ICardType,trnicrd.RequestId  FROM BasicDetails B " +
                           "inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId " +
                           "inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId " +
                           "inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
                           "inner join UserProfile pro on pro.ArmyNo=B.ServiceNo " +
                           "inner join MMappingProfile mpro on mpro.UserId=pro.UserId " +
                           "WHERE  mpro.GSOId = @UserId and C.Step = @stepcount ORDER BY B.UpdatedOn DESC";
            }
            else if (type == 3)///MI-11
            {
                query = "SELECT B.RegistrationType,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,B.PermanentAddress,C.Step StepCounter,C.Id StepId,ty.TypeId ICardType,trnicrd.RequestId " + 
                            "FROM BasicDetails B "+
                            "inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId "+
                            "inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId "+
                            "inner join MICardType ty on ty.TypeId = trnicrd.TypeId inner join UserProfile pro on pro.ArmyNo = B.ServiceNo "+
                            "inner join MMappingProfile mpro on mpro.UserId = pro.UserId "+
                            "inner join TrnFwds FWD ON FWD.RequestId = c.RequestId "+
                            "inner join MUnit mUNI ON mUNI.UnitId = FWD.SusNo "+
                            "WHERE mUNI.UnitId = @UserId AND C.Step = @stepcount ORDER BY B.UpdatedOn DESC";
            }
            else if (type == 4)///Hq-54
            {
                query = "SELECT B.RegistrationType,B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,B.PermanentAddress,C.Step StepCounter,C.Id StepId,ty.TypeId ICardType,trnicrd.RequestId  FROM BasicDetails B " +
                           "inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId " +
                           "inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId " +
                           "inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
                           "inner join UserProfile pro on pro.ArmyNo=B.ServiceNo " +
                           "inner join TrnFwds FWD ON FWD.RequestId = c.RequestId " +
                           "inner join MMappingProfile mpro on mpro.UserId = pro.UserId" +
                           " WHERE FWD.ToUserId = @UserId and C.Step = 5 ORDER BY B.UpdatedOn DESC";
            }


            using (var connection = _contextDP.CreateConnection())
            {
                var BasicDetailList = await connection.QueryAsync<DTOBasicDetailRequest>(query, new { UserId, stepcount });
                int sno = 1;
                var allrecord = (from e in BasicDetailList
                                 select new DTOBasicDetailRequest()
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
                                     RegistrationType = e.RegistrationType,
                                 }).ToList();
                return await Task.FromResult(allrecord);

            }

        }
    }
}
