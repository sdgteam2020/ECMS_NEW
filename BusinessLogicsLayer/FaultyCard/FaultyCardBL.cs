using BusinessLogicsLayer.FaultyStage;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
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
    public class FaultyCardBL : GenericRepositoryDL<TrnFaultyCard>, IFaultyCardBL
    {
        private readonly IFaultyCardDB _iFaultyCardDB;
        public FaultyCardBL(ApplicationDbContext context, IFaultyCardDB iFaultyCardDB) : base(context)
        {
            _iFaultyCardDB=iFaultyCardDB;
        }
        public async Task<DTOFaultyCardSaveResponse> SaveFaultyCard(DTOFaultyCardRequest dTO) 
        {
            return await _iFaultyCardDB.SaveFaultyCard(dTO);
        }
    }
}
