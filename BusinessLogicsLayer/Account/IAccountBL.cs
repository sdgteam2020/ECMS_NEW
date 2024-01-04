using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
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
        public Task<List<DTORegisterListRequest>> DomainApproveList();
        public Task<List<DTOProfileManageResponse>?> GetAllProfileManage(string Search, string Choice);
        public Task<List<DTOUserRegnResponse>?> GetAllUserRegn(string Search, string Choice);
    }
}
