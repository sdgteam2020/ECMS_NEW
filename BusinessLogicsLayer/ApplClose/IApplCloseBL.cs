using DataAccessLayer;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.Posting
{
    public interface IApplCloseBL : IGenericRepositoryDL<TrnApplClose>
    {
        public Task<DTOApplicationCloseResponse> RequestIdExists(DTOApplicationCloseRequest DTo);
        public Task<bool> ApplCloseWithUpdateStatus(TrnApplClose Data, ICardHistoryResponseAll? cardHistoryResponses);
        public Task<DTOGenericResponse<byte[]>> GenerateClosedHistoryPDF(ICardHistoryResponseAll? cardHistoryResponseAll, string ipAddress);
    }
}
