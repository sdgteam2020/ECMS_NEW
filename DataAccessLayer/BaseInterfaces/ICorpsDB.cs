using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace DataAccessLayer.BaseInterfaces
{
    public interface ICorpsDB : IGenericRepositoryDL<MCorps>
    {
        public Task<bool> GetByName(MCorps Data);
        public Task<List<DTOCorpsResponse>> GetALLCorps();
        public Task<List<DTOCorpsResponse>> GetByComdId(int ComdId);
        public Task<DTOCorpsIdCheckInFKTableResponse?> CorpsIdCheckInFKTable(byte CorpsId);
        public Task<DTODataTablesResponse<DTOCorpsResponse>> GetAllCorps_Pagination(DTODataTablesRequest dTO);
    }
}
