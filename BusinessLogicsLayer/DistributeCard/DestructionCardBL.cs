using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicsLayer.DestructionCard;
using BusinessLogicsLayer.FaultyCard;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.DistributeCard
{
    public class DestructionCardBL : GenericRepositoryDL<TrnDestructionCard>, IDestructionCardBL
    {
        private readonly IDestructionCardDB _iDestructionCardDB;
        public DestructionCardBL(ApplicationDbContext context, IDestructionCardDB iDestructionCardDB) : base(context)
        {
            _iDestructionCardDB = iDestructionCardDB;
        }
        public async Task<bool> FindAnyRequestId(int RequestId)
        {
            return await _iDestructionCardDB.FindAnyRequestId(RequestId);
        }
        public async Task<DTODataTablesResponse<DTODestructionCardGetResponse>> GetAllDestruction(DTODataTablesRequest dTO)
        {
            return await _iDestructionCardDB.GetAllDestruction(dTO);
        }
        public async Task<List<DTODestructionCardExportResponse>> GetDetailsByRequestIds(DTOHotlistCardsExportRequest Data)
        {
            return await _iDestructionCardDB.GetDetailsByRequestIds(Data);
        }
    }
}
