using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Model;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.CompletedICard
{
    public interface ICompletedICardRequestBL : IGenericRepositoryDL<CompletedICardRequest>
    {
        public Task<CompletedICardRequest?> GetByRequestId(int RequestId);
        public Task<DTOGenericResponse<byte[]>> GetCompletedHistoryPdf(ICardHistoryResponseAll responseAll, string ipAddress);
    }
}
