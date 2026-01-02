using DataAccessLayer;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.Corps
{
    public interface ICorpsBL : IGenericRepositoryDL<MCorps>
    {

        public Task<bool> GetByName(MCorps Data);
        public Task<List<DTOCorpsResponse>> GetByComdId(int ComdId);
        public Task<List<DTOCorpsResponse>> GetALLCorps();
        public Task<DTOCorpsIdCheckInFKTableResponse?> CorpsIdCheckInFKTable(byte CorpsId);
        public Task<DTODataTablesResponse<DTOCorpsResponse>> GetAllCorps_Pagination(DTODataTablesRequest dTO);
    }
}
