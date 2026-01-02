using DataAccessLayer;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.Bde
{
    public interface IBdeBL : IGenericRepositoryDL<MBde>
    {

        public Task<bool?> GetByName(MBde Data);
        public Task<List<DTOBdeResponse>> GetALLBdeCat();
        public Task<List<DTOBdeResponse>> GetByHId(DTOParentChildIdRequest Data);
        public Task<bool?> FindByBdeWithId(string BdeName, byte BdeId);
        public Task<DTOBdeIdCheckInFKTableResponse?> BdeIdCheckInFKTable(byte BdeId);
        public Task<DTODataTablesResponse<DTOBdeResponse>> GetAllBde_Pagination(DTODataTablesRequest dTO);

    }
}
