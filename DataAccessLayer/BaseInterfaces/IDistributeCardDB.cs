using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IDistributeCardDB : IGenericRepositoryDL<TrnDistributeCard>
    {
        Task<bool> FindAnyRequestId(int RequestId);
        Task<DTODataTablesResponse<DTODistributeCardGetResponse>> GetAllDistribute(DTODataTablesRequest dTO);
        Task<List<DTODistributeCardExportResponse>> GetDetailsByRequestIds(DTOHotlistCardsExportRequest Data);
        Task<DTOCommonSaveResponse> SaveDistributeCard(TrnDistributeCard model, ICardHistoryResponseAll cardRequestHistory);

    }
}
