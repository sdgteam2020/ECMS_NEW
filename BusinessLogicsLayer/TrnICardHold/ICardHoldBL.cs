using BusinessLogicsLayer.Bde;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Model;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.TrnICardHold
{
    public class ICardHoldBL : GenericRepositoryDL<MTrnICardHold>, IICardHoldBL
    {
        private readonly IICardHoldDB _iICardHoldDB;
        public ICardHoldBL(ApplicationDbContext context, IICardHoldDB iICardHoldDB) : base(context)
        {
            _iICardHoldDB = iICardHoldDB;
        }
        public async Task<bool> GetByRequestId(MTrnICardHold dTO)
        {
            return await _iICardHoldDB.GetByRequestId(dTO);
        }
        public async Task<DTOBeforeSaveICardRequestHoldResponse> CheckBeforeICardRequestHold(MTrnICardHold hold)
        {
            return await _iICardHoldDB.CheckBeforeICardRequestHold(hold);
        }
    }
}
