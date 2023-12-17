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

namespace DataAccessLayer
{
    public class AccountDB : GenericRepositoryDL<ApplicationUser>, IAccountDB
    {
        protected new readonly ApplicationDbContext _context;
        public AccountDB(ApplicationDbContext context) : base(context)
        {
            _context = context;
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
    }
}
