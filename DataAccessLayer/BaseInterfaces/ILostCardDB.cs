using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace DataAccessLayer.BaseInterfaces
{
    public interface ILostCardDB : IGenericRepositoryDL<TrnLostCard>
    {
        Task<bool> FindAnyRequestId(int RequestId);
        public Task<bool> CheckServiceNoRequestInLost(string ServiceNo);
        Task<DTODataTablesWithSelectedIdsResponse<DTOLostCardGetResponse>> GetAllLost(DTODataTablesRequestForCommanCheckAll dTO);
        Task<List<DTOLostCardExportResponse>> GetDetailsByRequestIds(DTOHotlistCardsExportRequest Data);
        public Task<DTOCheckBeforeLostReportResponse> CheckBeforeLostReport(int requestId, int TDMId);
        public Task<DTOGenericResponse<DTOCommonResponse?>> SaveLostCardRequest(DTOLostCardAddRequest Data, ICardHistoryResponseAll? cardHistoryResponses);
    }
}
