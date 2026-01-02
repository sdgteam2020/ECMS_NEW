using DataAccessLayer;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.Div
{
    public interface IDivBL : IGenericRepositoryDL<MDiv>
    {

        public Task<bool> GetByName(MDiv Data);
        public Task<List<DTODivResponse>> GetALLDiv(); 
        public Task<List<DTODivResponse>> GetByHId(DTOParentChildIdRequest Data);
        public Task<DTODivIdCheckInFKTableResponse?> DivIdCheckInFKTable(byte DivId);
        public Task<DTODataTablesResponse<DTODivResponse>> GetAllDiv_Pagination(DTODataTablesRequest dTO);
    }
}

