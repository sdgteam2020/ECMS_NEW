using DataAccessLayer;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.Master
{
    public interface IArmedBL : IGenericRepositoryDL<MArmedType>
    {

        public Task<bool> GetByName(MArmedType Dto);
        public Task<List<DTOArmedResponse>> GetALLArmed();
        public Task<DTOArmedIdCheckInFKTableResponse?> ArmedIdCheckInFKTable(byte ArmedId);
        public Task<DTODataTablesResponse<DTOArmedResponse>> GetAllArmed_Pagination(DTODataTablesRequest dTO);
    }
}
