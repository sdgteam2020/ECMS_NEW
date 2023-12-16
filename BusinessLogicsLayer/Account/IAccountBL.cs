using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Account
{
    public interface IAccountBL : IGenericRepository<ApplicationUser>
    {
        public Task<DTOAccountResponse?> FindDomainId(string DomainId);
    }
}
