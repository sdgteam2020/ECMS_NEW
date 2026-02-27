using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IDestructionCardDB : IGenericRepositoryDL<TrnDestructionCard>
    {
        Task<bool> FindAnyRequestId(int RequestId);
        Task<DTODataTablesWithSelectedIdsResponse<DTODestructionCardGetResponse>> GetAllDestruction(DTODataTablesRequestForCommanCheckAll dTO);
        Task<List<DTODestructionCardExportResponse>> GetDetailsByRequestIds(DTOHotlistCardsExportRequest Data);
        public Task<DTOGenericResponse<string>> CheckBeforeDestructionCardReport(int RequestId);

    }
}
