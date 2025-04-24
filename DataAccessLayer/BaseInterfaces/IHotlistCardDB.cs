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
    public interface IHotlistCardDB : IGenericRepositoryDL<TrnHotlistCard>
    {
        Task<bool> FindAnyRequestId(int RequestId);
        Task<DTODataTablesResponse<DTOHotlistCardGetResponse>> GetAllHotlist(DTODataTablesRequest dTO);
        Task<List<DTOHotlistCardExportResponse>> GetDetailsByRequestIds(DTOHotlistCardsExportRequest Data);

    }
}
