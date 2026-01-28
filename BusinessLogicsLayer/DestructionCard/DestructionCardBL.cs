using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.DestructionCard
{
    public class DestructionCardBL : GenericRepositoryDL<TrnDestructionCard>, IDestructionCardBL
    {
        private readonly IDestructionCardDB _iDestructionCardDB;
        public DestructionCardBL(ApplicationDbContext context, IDestructionCardDB iDestructionCardDB) : base(context)
        {
            _iDestructionCardDB = iDestructionCardDB;
        }
        public async Task<bool> FindAnyRequestId(int RequestId)
        {
            return await _iDestructionCardDB.FindAnyRequestId(RequestId);
        }
        public async Task<DTODataTablesWithSelectedIdsResponse<DTODestructionCardGetResponse>> GetAllDestruction(DTODataTablesRequestForCommanCheckAll dTO)
        {
            return await _iDestructionCardDB.GetAllDestruction(dTO);
        }
        public async Task<List<DTODestructionCardExportResponse>> GetDetailsByRequestIds(DTOHotlistCardsExportRequest Data)
        {
            return await _iDestructionCardDB.GetDetailsByRequestIds(Data);
        }
    }
}
