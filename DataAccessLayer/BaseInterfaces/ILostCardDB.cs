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
    public interface ILostCardDB : IGenericRepositoryDL<TrnLostCard>
    {
        Task<bool> FindAnyRequestId(int RequestId);
        public Task<bool> CheckServiceNoRequestInLost(string ServiceNo);
        Task<DTODataTablesResponse<DTOLostCardGetResponse>> GetAllLost(DTODataTablesRequest dTO);
        Task<List<DTOLostCardExportResponse>> GetDetailsByRequestIds(DTOHotlistCardsExportRequest Data);
    }
}
