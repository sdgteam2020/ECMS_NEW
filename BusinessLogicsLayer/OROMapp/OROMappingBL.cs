using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.OROMapp
{
    public class OROMappingBL : GenericRepositoryDL<OROMapping>, IOROMappingBL
    {
        private readonly IOROMappingDB _OROMappingDB;
        public OROMappingBL(ApplicationDbContext context, IOROMappingDB iOROMappingDB) : base(context)
        {
            _OROMappingDB = iOROMappingDB;
        }
        public async Task<List<DTOOROMappingResponse>?> GetAllOROMapping()
        {
            return await _OROMappingDB.GetAllOROMapping();
        }
        public async Task<bool> GetByName(OROMapping Dto)
        {
            return await _OROMappingDB.GetByName(Dto);
        }
        public async Task<List<DTOAllOROResponse>> GetAllORO()
        {
            return await _OROMappingDB.GetAllORO();
        }
        public async Task<DTODataTablesResponse<DTOOROMappingResponse>> GetAllOROMapping_Pagination(DTODataTablesRequest dTO)
        {
            return await _OROMappingDB.GetAllOROMapping_Pagination(dTO);
        }
    }
}
