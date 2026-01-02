using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IArmedDB : IGenericRepositoryDL<MArmedType>
    {
        public Task<bool> GetByName(MArmedType Dto);
        public Task<List<DTOArmedResponse>> GetALLArmed();
        public Task<DTOArmedIdCheckInFKTableResponse?> ArmedIdCheckInFKTable(byte ArmedId);
        public Task<DTODataTablesResponse<DTOArmedResponse>> GetAllArmed_Pagination(DTODataTablesRequest dTO);
    }
}
