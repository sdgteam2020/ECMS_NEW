using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataTransferObject.Requests;
using Microsoft.AspNetCore.DataProtection;

namespace DataAccessLayer
{
    public class AccountDB : GenericRepositoryDL<ApplicationUser>, IAccountDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly IDataProtector protector;
        public AccountDB(ApplicationDbContext context, IDataProtectionProvider dataProtectionProvider, DataProtectionPurposeStrings dataProtectionPurposeStrings) : base(context)
        {
            _context = context;
            // Pass the purpose string as a parameter
            this.protector = dataProtectionProvider.CreateProtector(
                dataProtectionPurposeStrings.AFSACIdRouteValue);
        }
        public async Task<DTOAccountResponse?> FindDomainId(string DomainId)
        {
            var applicationUser = await _context.Users.Where(x => x.DomainId == DomainId).Select(x=> new { x.Id, x.DomainId,x.Active,x.AdminFlag }).FirstOrDefaultAsync();
            if(applicationUser!=null)
            {
                DTOAccountResponse dTOAccountResponse = new DTOAccountResponse();
                dTOAccountResponse.Id= applicationUser.Id;
                dTOAccountResponse.DomainId= applicationUser.DomainId;
                dTOAccountResponse.Active = applicationUser.Active;
                dTOAccountResponse.AdminFlag = applicationUser.AdminFlag;
                return dTOAccountResponse;
            }
            else
            {
                return null;
            }

        }
        public async Task<List<DTORegisterListRequest>> DomainApproveList()
        {
            try
            {
                var allrecord = await (from e in _context.Users
                                       join r in _context.UserRoles on e.Id equals r.UserId
                                       join n in _context.Roles on r.RoleId equals n.Id
                                       where e.AdminFlag == false
                                       orderby e.Id
                                       select new DTORegisterListRequest()
                                       {
                                           EncryptedId = protector.Protect(e.Id.ToString()),
                                           DomainId = e.DomainId,
                                           RoleName = n.Name != null ? n.Name : "no role assign",
                                       }).ToListAsync();
                return allrecord;
            }
            catch (Exception ex) 
            {
                return null;
            }

        }
    }
}
