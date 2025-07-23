using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.DestructionCard
{
    public interface IDestructionCardBL : IGenericRepositoryDL<TrnDestructionCard>
    {
        Task<bool> FindAnyRequestId(int RequestId);
        Task<DTODataTablesResponse<DTODestructionCardGetResponse>> GetAllDestruction(DTODataTablesRequest dTO);
        Task<List<DTODestructionCardExportResponse>> GetDetailsByRequestIds(DTOHotlistCardsExportRequest Data);
    }
}
