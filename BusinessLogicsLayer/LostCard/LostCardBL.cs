using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.LostCard
{
    public class LostCardBL : GenericRepositoryDL<TrnLostCard>, ILostCardBL
    {
        private readonly ILostCardDB _iLostCardDB;
        public LostCardBL(ApplicationDbContext context, ILostCardDB iLostCardDB) : base(context)
        {
            _iLostCardDB = iLostCardDB;
        }

        public async Task<bool> FindAnyRequestId(int RequestId)
        {
            return await _iLostCardDB.FindAnyRequestId(RequestId);
        }

        public async Task<DTODataTablesResponse<DTOLostCardGetResponse>> GetAllLost(DTODataTablesRequest dTO)
        {
            return await _iLostCardDB.GetAllLost(dTO);
        }

        public async Task<List<DTOLostCardExportResponse>> GetDetailsByRequestIds(DTOHotlistCardsExportRequest Data)
        {
            return await _iLostCardDB.GetDetailsByRequestIds(Data);
        }
    }
}
