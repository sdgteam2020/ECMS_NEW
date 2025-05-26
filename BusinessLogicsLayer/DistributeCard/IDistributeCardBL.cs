using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.DistributeCard
{
    public interface IDistributeCardBL : IGenericRepository<TrnDistributeCard>
    {
        Task<bool> FindRequestId(int RequestId);
        Task<DTODataTablesResponse<DTODistributeCardGetResponse>> GetAllDistribute(DTODataTablesRequest dTO);
        Task<List<DTODistributeCardExportResponse>> GetDetailsByRequestIds(DTOHotlistCardsExportRequest Data);
        Task<DTOCommonSaveResponse> SaveDistributeCard(TrnDistributeCard model, ICardHistoryResponseAll cardRequestHistory);
    }
}
