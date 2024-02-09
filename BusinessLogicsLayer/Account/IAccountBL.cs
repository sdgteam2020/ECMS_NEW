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
        public Task<int> TotalProfileCount();
        public bool GetByDomainId(string DomainId, int Id);
        public Task<DTOAccountResponse?> FindDomainId(string DomainId);
        public Task<bool?> FindRoleByName(string Role);
        public Task<List<DTORegisterListRequest>> DomainApproveList();
        public Task<List<DTOProfileManageResponse>?> GetAllProfileManage(string Search, string Choice);
        public Task<List<DTOUserRegnResponse>?> GetAllUserRegn(string Search, string Choice);
        public Task<List<DTODomainRegnResponse>?> GetAllDomainRegn(string Search, string Choice);
        public Task<DTOUserRegnResultResponse?> SaveMapping(DTOUserRegnMappingRequest dTO);
        public Task<bool?> SaveDomainRegn(DTODomainRegnRequest dTO);
        public Task<bool?> UpdateDomainFlag(DTOUserRegnUpdateDomainFlagRequest dTO);
        public Task<List<DTOMasterResponse>> GetAllRole();
        public Task<DTOTempSession?> ProfileAndMappingSaving(DTOProfileAndMappingRequest model, DTOTempSession dTOTempSession);
        public Task<DTOAccountCountResponse> AccountCount();
        public Task<bool?> SaveUnitWithMapping(DTOSaveUnitWithMappingRequest dTO);
        public Task<DTODataTablesResponse<DTOUserRegnResponse>> GetDataForDataTable(DTODataTablesRequest request);
    }
}
