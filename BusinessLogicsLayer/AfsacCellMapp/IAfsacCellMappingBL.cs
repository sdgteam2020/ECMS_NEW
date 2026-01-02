using DataAccessLayer;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.AfsacCellMapp
{
    public interface IAfsacCellMappingBL : IGenericRepositoryDL<AfsacCellMapping>
    {
        public Task<List<DTOAfsacCellMappingResponse>?> GetAllAfsacCellMapping();
        public Task<bool> GetByName(AfsacCellMapping Dto);
        public Task<DTODataTablesResponse<DTOAfsacCellMappingResponse>> GetAllAfsacCellMapping_Pagination(DTODataTablesRequest dTO);
    }
}
