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

namespace BusinessLogicsLayer.HotlistCard
{
    public class HotlistCardBL : GenericRepositoryDL<TrnHotlistCard>, IHotlistCardBL
    {
        private readonly IHotlistCardDB _iHotlistCardDB;
        public HotlistCardBL(ApplicationDbContext context, IHotlistCardDB iHotlistCardDB) : base(context)
        {
            _iHotlistCardDB = iHotlistCardDB;
        }
        public async Task<bool> FindRequestId(int RequestId)
        {
            return await _iHotlistCardDB.FindAnyRequestId(RequestId);
        }
        public async Task<DTODataTablesResponse<DTOHotlistCardGetResponse>> GetAllHotlist(DTODataTablesRequest dTO)
        {
            return await _iHotlistCardDB.GetAllHotlist(dTO);
        }
        public async Task<List<DTOHotlistCardExportResponse>> GetDetailsByRequestIds(DTOHotlistCardsExportRequest Data)
        {
            return await _iHotlistCardDB.GetDetailsByRequestIds(Data);
        }
    }
}
