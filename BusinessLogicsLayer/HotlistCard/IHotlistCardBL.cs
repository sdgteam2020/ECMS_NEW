using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.HotlistCard
{
    public interface IHotlistCardBL : IGenericRepository<TrnHotlistCard>
    {
        Task<bool> FindRequestId(int RequestId);
        Task<DTODataTablesResponse<DTOHotlistCardGetResponse>> GetAllHotlist(DTODataTablesRequest dTO);
        Task<List<DTOHotlistCardExportResponse>> GetDetailsByRequestIds(DTOHotlistCardsExportRequest Data);
    }
}
