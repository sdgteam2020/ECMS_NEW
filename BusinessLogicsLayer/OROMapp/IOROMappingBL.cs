using DataAccessLayer;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.OROMapp
{
    public interface IOROMappingBL : IGenericRepositoryDL<OROMapping>
    {
        public Task<List<DTOOROMappingResponse>?> GetAllOROMapping();
        public Task<bool> GetByName(OROMapping Dto);
        public Task<List<DTOAllOROResponse>> GetAllORO();
        public Task<DTODataTablesResponse<DTOOROMappingResponse>> GetAllOROMapping_Pagination(DTODataTablesRequest dTO);
    }
}
