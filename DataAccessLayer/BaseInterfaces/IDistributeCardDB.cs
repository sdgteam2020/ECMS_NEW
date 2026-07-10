using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IDistributeCardDB : IGenericRepositoryDL<TrnDistributeCard>
    {
        Task<bool> FindAnyRequestId(int RequestId);
        Task<DTODataTablesWithSelectedIdsResponse<DTODistributeCardGetResponse>> GetAllDistribute(DTODataTablesRequestForCommanCheckAll dTO);
        Task<List<DTODistributeCardExportResponse>> GetDetailsByRequestIds(DTOHotlistCardsExportRequest Data);
        Task<DTOCommonSaveResponse> SaveDistributeCard(TrnDistributeCard model, ICardHistoryResponseAll cardRequestHistory, List<int> AspNetUsersIds);

    }
}
