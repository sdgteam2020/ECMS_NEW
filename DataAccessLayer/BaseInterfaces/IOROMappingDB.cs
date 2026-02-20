using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IOROMappingDB : IGenericRepositoryDL<OROMapping>
    {
        public Task<List<DTOOROMappingResponse>?> GetAllOROMapping();
        public Task<bool> GetByName(OROMapping Dto);
        public Task<List<DTOAllOROResponse>> GetAllORO();
        public Task<DTODataTablesResponse<DTOOROMappingResponse>> GetAllOROMapping_Pagination(DTODataTablesRequest dTO);
        public Task<bool> ValidateTDMIdInOROMapping(int TDMId);
    }
}
