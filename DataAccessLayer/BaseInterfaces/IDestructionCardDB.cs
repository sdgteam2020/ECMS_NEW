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
    public interface IDestructionCardDB : IGenericRepositoryDL<TrnDestructionCard>
    {
        Task<bool> FindAnyRequestId(int RequestId);
        Task<DTODataTablesResponse<DTODestructionCardGetResponse>> GetAllDestruction(DTODataTablesRequest dTO);
        Task<List<DTODestructionCardExportResponse>> GetDetailsByRequestIds(DTOHotlistCardsExportRequest Data);

    }
}
