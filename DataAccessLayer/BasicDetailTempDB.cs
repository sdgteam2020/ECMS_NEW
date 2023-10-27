using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class BasicDetailTempDB : GenericRepositoryDL<BasicDetailTemp>, IBasicDetailTempDB
    {
        protected readonly ApplicationDbContext _context;
        private readonly IDataProtector protector;
        public BasicDetailTempDB(ApplicationDbContext context, IDataProtectionProvider dataProtectionProvider, DataProtectionPurposeStrings dataProtectionPurposeStrings) : base(context)
        {
            _context = context;
            // Pass the purpose string as a parameter
            this.protector = dataProtectionProvider.CreateProtector(
                dataProtectionPurposeStrings.AFSACIdRouteValue);
            this.protector = protector;
        }
        public Task<List<DTOBasicDetailTempRequest>> GetALLBasicDetailTemp(int UserId)
        {
            int sno = 1;
            var BasicDetailTempList = _context.BasicDetailTemps.Where(x => x.Updatedby == UserId).ToList();
            var allrecord = (from e in BasicDetailTempList
                             orderby e.UpdatedOn descending
                             select new DTOBasicDetailTempRequest()
                             {
                                 EncryptedId = protector.Protect(e.BasicDetailTempId.ToString()),
                                 Sno = sno++,
                                 Name = e.Name,
                                 ServiceNo = e.ServiceNo,
                                 DOB = e.DOB,
                                 DateOfCommissioning = e.DateOfCommissioning,
                                 PermanentAddress = e.PermanentAddress,
                             }).ToList();
            return Task.FromResult(allrecord);
        }
    }
}
