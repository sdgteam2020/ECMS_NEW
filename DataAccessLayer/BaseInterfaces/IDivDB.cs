using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IDivDB : IGenericRepositoryDL<MDiv>
    {
        public Task<bool> GetByName(MDiv Data);
        public Task<List<DTODivResponse>> GetALLDiv();
        public Task<List<DTODivResponse>> GetByHId(DTOParentChildIdRequest Data);
        public Task<DTODivIdCheckInFKTableResponse?> DivIdCheckInFKTable(byte DivId);
    }
   
}
