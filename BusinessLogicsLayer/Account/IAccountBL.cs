using DataAccessLayer;
using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.Account
{
    public interface IAccountBL : IGenericRepositoryDL<ApplicationUser>
    {
        public Task<int> TotalProfileCount();
        public bool GetByDomainId(string DomainId, int Id);
        public Task<DTOAccountResponse?> FindDomainId(string DomainId);
        public Task<bool?> FindRoleByName(string Role);
        public Task<DTODataTablesResponse<DTOUserRegnResponse>> GetAllUserRegn(DTODataTablesRequest request);
        public Task<DTODataTablesResponse<DTODomainRegnResponse>> GetAllDomainRegn(DTODataTablesRequest request);
        public Task<DTODataTablesResponse<DTOProfileManageResponse>> GetAllProfileManage(DTODataTablesRequest request);
        public Task<DTOUserRegnResultResponse?> SaveMapping(DTOUserRegnMappingRequest dTO);
        public Task<bool?> SaveDomainRegn(DTODomainRegnRequest dTO);
        public Task<bool?> UpdateDomainFlag(DTOUserRegnUpdateDomainFlagRequest dTO);
        public Task<List<DTOMasterResponse>> GetAllRole();
        public Task<List<DTOClaimsResponse>> GetAllClaims();
        public Task<DTOTempSession?> ProfileAndMappingSaving(DTOProfileAndMappingRequest model, DTOTempSession dTOTempSession);
        public Task<DTOAccountCountResponse> AccountCount();
        public Task<bool> SaveUnitWithMapping(DTOSaveUnitWithMappingRequest dTO);
        public Task<DTODataTablesResponse<DTOUserRegnResponse>> GetDataForDataTable(DTODataTablesRequest request);
        Task<DTODataTablesResponse<DTOClaimsStoreResponse>?> GetAllClaimsOrderBy(DTODataTablesRequest request);
        Task<DTODataTablesResponse<DTOUsersByClaim>> GetAllUsersByClaim(DTODataTablesRequest request);
    }
}
