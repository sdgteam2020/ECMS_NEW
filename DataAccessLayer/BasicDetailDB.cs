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

namespace DataAccessLayer
{
    public class BasicDetailDB:GenericRepositoryDL<BasicDetail>,IBasicDetailDB
    {
        protected readonly ApplicationDbContext _context;
        private readonly IDataProtector protector;
        public BasicDetailDB(ApplicationDbContext context, IDataProtectionProvider dataProtectionProvider, DataProtectionPurposeStrings dataProtectionPurposeStrings) : base(context)
        {
            _context = context;
            // Pass the purpose string as a parameter
            this.protector = dataProtectionProvider.CreateProtector(
                dataProtectionPurposeStrings.AFSACIdRouteValue);
            this.protector = protector;
        }
        public Task<List<DTOBasicDetailRequest>> GetALLBasicDetail(int UserId)
        {
            int sno = 1;
            var BasicDetailList = _context.BasicDetails.Where(x => x.IsDeleted == false && x.Updatedby == UserId).ToList();
            var allrecord = (from e in BasicDetailList
                            orderby e.UpdatedOn descending
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
                            }).ToList();
            return Task.FromResult(allrecord);
        }
    }
}
