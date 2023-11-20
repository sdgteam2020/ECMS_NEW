using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;


namespace DataAccessLayer
{
    public class BasicDetailTempDB : GenericRepositoryDL<BasicDetailTemp>, IBasicDetailTempDB
    {
        protected readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly IDataProtector protector;
        public BasicDetailTempDB(ApplicationDbContext context, DapperContext contextDP, IDataProtectionProvider dataProtectionProvider, DataProtectionPurposeStrings dataProtectionPurposeStrings) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            // Pass the purpose string as a parameter
            this.protector = dataProtectionProvider.CreateProtector(
                dataProtectionPurposeStrings.AFSACIdRouteValue);
            this.protector = protector;
        }
        public async Task<List<DTOBasicDetailTempRequest>> GetALLBasicDetailTemp(int UserId)
        {
            //var BasicDetailTempList = _context.BasicDetailTemps.Where(x => x.Updatedby == UserId).ToList();
            var query = "SELECT * FROM BasicDetailTemps WHERE Updatedby=@UserId ORDER BY UpdatedOn DESC";
            using (var connection = _contextDP.CreateConnection())
            {
                var BasicDetailTempList = await connection.QueryAsync<BasicDetailTemp>(query, new { UserId });
                int sno = 1;
                var allrecord = (from e in BasicDetailTempList
                                 select new DTOBasicDetailTempRequest()
                                 {
                                     EncryptedId = protector.Protect(e.BasicDetailTempId.ToString()),
                                     Sno = sno++,
                                     Name = e.Name,
                                     ServiceNo = e.ServiceNo,
                                     DOB = e.DOB,
                                     DateOfCommissioning = e.DateOfCommissioning,
                                     PermanentAddress = e.PermanentAddress,
                                     Observations = e.Observations,
                                 }).ToList();
                return await Task.FromResult(allrecord);

            }

        }
    }
}
