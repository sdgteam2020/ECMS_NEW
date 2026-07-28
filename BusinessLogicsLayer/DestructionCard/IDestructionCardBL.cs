using DataAccessLayer;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.DestructionCard
{
    public interface IDestructionCardBL : IGenericRepositoryDL<TrnDestructionCard>
    {
        Task<bool> FindAnyRequestId(int RequestId);
        Task<DTODataTablesWithSelectedIdsResponse<DTODestructionCardGetResponse>> GetAllDestruction(DTODataTablesRequestForCommanCheckAll dTO);
        Task<List<DTODestructionCardExportResponse>> GetDetailsByRequestIds(DTOHotlistCardsExportRequest Data);
        public Task<DTOCheckBeforeDestructionCardReportResponse> CheckBeforeDestructionCardReport(int RequestId);
        public Task<DTOGenericResponse<DTOCommonResponse?>> SaveDestructionCardRequest(TrnDestructionCard Data, DTOCheckBeforeDestructionCardReportResponse checkCardBeforeDistruction);
    }
}
