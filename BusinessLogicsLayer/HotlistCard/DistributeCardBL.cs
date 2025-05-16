using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicsLayer.FaultyCard;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.DistributeCard
{
    public class DistributeCardBL : GenericRepositoryDL<TrnDistributeCard>, IDistributeCardBL
    {
        private readonly IDistributeCardDB _iDistributeCardDB;
        public DistributeCardBL(ApplicationDbContext context, IDistributeCardDB iDistributeCardDB) : base(context)
        {
            _iDistributeCardDB = iDistributeCardDB;
        }
        public async Task<bool> FindRequestId(int RequestId)
        {
            return await _iDistributeCardDB.FindAnyRequestId(RequestId);
        }
        public async Task<DTODataTablesResponse<DTODistributeCardGetResponse>> GetAllDistribute(DTODataTablesRequest dTO)
        {
            return await _iDistributeCardDB.GetAllDistribute(dTO);
        }
        public async Task<List<DTODistributeCardExportResponse>> GetDetailsByRequestIds(DTOHotlistCardsExportRequest Data)
        {
            return await _iDistributeCardDB.GetDetailsByRequestIds(Data);
        }
    }
}
