using DataAccessLayer;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.DistributeCard
{
    public interface IDistributeCardBL : IGenericRepositoryDL<TrnDistributeCard>
    {
        Task<bool> FindRequestId(int RequestId);
        Task<DTODataTablesWithSelectedIdsResponse<DTODistributeCardGetResponse>> GetAllDistribute(DTODataTablesRequestForCommanCheckAll dTO);
        Task<List<DTODistributeCardExportResponse>> GetDetailsByRequestIds(DTOHotlistCardsExportRequest Data);
        Task<DTOCommonSaveResponse> SaveDistributeCard(TrnDistributeCard model, ICardHistoryResponseAll cardRequestHistory);
    }
}
