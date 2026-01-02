using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.Div
{
    public class DivBL : GenericRepositoryDL<MDiv>, IDivBL
    {
        private readonly IDivDB _DivDB;

        public DivBL(ApplicationDbContext context, IDivDB sqnDB) : base(context)
        {
            _DivDB = sqnDB;
        }
        public Task<List<DTODivResponse>> GetALLDiv()
        {
            return _DivDB.GetALLDiv();
        }

        public Task<List<DTODivResponse>> GetByHId(DTOParentChildIdRequest Data)
        {
            return _DivDB.GetByHId(Data);
        }

        public Task<bool> GetByName(MDiv Data)
        {
            return _DivDB.GetByName(Data);
        }
        public async Task<DTODivIdCheckInFKTableResponse?> DivIdCheckInFKTable(byte DivId)
        {
            return await _DivDB.DivIdCheckInFKTable(DivId);
        }
        public async Task<DTODataTablesResponse<DTODivResponse>> GetAllDiv_Pagination(DTODataTablesRequest dTO)
        {
            return await _DivDB.GetAllDiv_Pagination(dTO);
        }
    }
}
