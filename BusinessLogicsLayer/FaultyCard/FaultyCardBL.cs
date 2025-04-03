using Azure.Core;
using BusinessLogicsLayer.FaultyStage;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.FaultyCard
{
    public class FaultyCardBL : GenericRepositoryDL<TrnFaultyCard>, IFaultyCardBL
    {
        private readonly IFaultyCardDB _iFaultyCardDB;
        public FaultyCardBL(ApplicationDbContext context, IFaultyCardDB iFaultyCardDB) : base(context)
        {
            _iFaultyCardDB=iFaultyCardDB;
        }
        public async Task<bool> FindRequestId(int RequestId)
        {
            return await _iFaultyCardDB.FindRequestId(RequestId);
        }
        public async Task<List<DTOFaultyCardListResponse>?> GetAllFaulty(bool Claim, int MapUnitId)
        {
            return await _iFaultyCardDB.GetAllFaulty(Claim, MapUnitId);
        }
        public async Task<DTOFaultyCardSaveResponse> SaveFaultyCard(DTOFaultyCardRequest dTO) 
        {
            return await _iFaultyCardDB.SaveFaultyCard(dTO);
        }
    }
}
