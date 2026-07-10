using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.DistributeCard
{
    public class DistributeCardBL : GenericRepositoryDL<TrnDistributeCard>, IDistributeCardBL
    {
        private readonly IDistributeCardDB _iDistributeCardDB;
        public DistributeCardBL(ApplicationDbContext context, IDistributeCardDB iDistributeCardDB) : base(context)
        {
            _iDistributeCardDB = iDistributeCardDB;
        }
        public async Task<bool> FindRequestId(int RequestId)
        {
            return await _iDistributeCardDB.FindAnyRequestId(RequestId);
        }
        public async Task<DTODataTablesWithSelectedIdsResponse<DTODistributeCardGetResponse>> GetAllDistribute(DTODataTablesRequestForCommanCheckAll dTO)
        {
            return await _iDistributeCardDB.GetAllDistribute(dTO);
        }
        public async Task<List<DTODistributeCardExportResponse>> GetDetailsByRequestIds(DTOHotlistCardsExportRequest Data)
        {
            return await _iDistributeCardDB.GetDetailsByRequestIds(Data);
        }
        
        public async Task<DTOCommonSaveResponse> SaveDistributeCard(TrnDistributeCard model, ICardHistoryResponseAll cardRequestHistory)
        {
            List<int> AspNetUsersIds = cardRequestHistory.ICardHistory?
                                                        .Where(x => x.StepId == 2)
                                                        .Select(x => x.ToAspNetUsersId)
                                                        .Distinct()
                                                        .ToList() ?? new List<int>();
            return await _iDistributeCardDB.SaveDistributeCard(model, cardRequestHistory, AspNetUsersIds);
        }
    }
}
