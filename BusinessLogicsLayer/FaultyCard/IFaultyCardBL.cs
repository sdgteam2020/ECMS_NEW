using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.FaultyCard
{
    public interface IFaultyCardBL : IGenericRepository<TrnFaultyCard>
    {
        public Task<bool> FindRequestId(int RequestId);
        public Task<DTOFaultyCardListResponse?> GetTrnFaultyCardDetail(int TrnFaultyCardId);
        public Task<List<DTOFaultyCardListResponse>?> GetAllFaulty(bool Claim,int MapUnitId);
        public Task<DTOFaultyCardSaveResponse> SaveFaultyCard(DTOFaultyCardRequest dTO, MTrnFwd? mTrnFwd);
    }
}
