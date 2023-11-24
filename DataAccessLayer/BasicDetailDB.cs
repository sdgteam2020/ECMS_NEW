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
        public async Task<List<DTOBasicDetailRequest>> GetALLBasicDetail(int UserId)
        {
            //var BasicDetailList = _context.BasicDetails.Where(x => x.IsDeleted == false && x.Updatedby == UserId).ToList();
            var query = "SELECT B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,B.PermanentAddress,C.Step StepCounter,C.Id StepId,ty.ICardType,trnicrd.RequestId  FROM BasicDetails B " + 
            "inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId "+ 
            "inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId "+ 
            "inner join MICardType ty on ty.TypeId = trnicrd.TypeId "+
            "WHERE B.Updatedby = @UserId and C.Step = 1 ORDER BY B.UpdatedOn DESC";

            using (var connection = _contextDP.CreateConnection())
            {
                var BasicDetailList = await connection.QueryAsync<DTOBasicDetailRequest>(query, new { UserId });
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
                                     ICardType=e.ICardType
                                 }).ToList();
                return await Task.FromResult(allrecord);

            }

        }
    }
}
