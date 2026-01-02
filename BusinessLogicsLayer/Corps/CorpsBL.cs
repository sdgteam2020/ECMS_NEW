using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.Corps
{
    public class CorpsBL : GenericRepositoryDL<MCorps>, ICorpsBL
    {
        private readonly ICorpsDB _iCorpsDB;

        public CorpsBL(ApplicationDbContext context, ICorpsDB corpsDB) : base(context)
        {
            _iCorpsDB = corpsDB;
        }

        public Task<List<DTOCorpsResponse>> GetALLCorps()
        {
            return _iCorpsDB.GetALLCorps();
        }

        public Task<List<DTOCorpsResponse>> GetByComdId(int ComdId)
        {
            return _iCorpsDB.GetByComdId(ComdId);
        }

        public Task<bool> GetByName(MCorps Data)
        {
           return _iCorpsDB.GetByName(Data);
        }
        public async Task<DTOCorpsIdCheckInFKTableResponse?> CorpsIdCheckInFKTable(byte CorpsId)
        {
            return await _iCorpsDB.CorpsIdCheckInFKTable(CorpsId);
        }
        public async Task<DTODataTablesResponse<DTOCorpsResponse>> GetAllCorps_Pagination(DTODataTablesRequest dTO)
        {
            return await _iCorpsDB.GetAllCorps_Pagination(dTO);
        }
    }
}
