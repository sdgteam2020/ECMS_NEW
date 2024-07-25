using DataTransferObject.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.TrnICardHold
{
    public interface IICardHoldBL: IGenericRepository<MTrnICardHold>
    {
        public Task<bool> GetByRequestId(MTrnICardHold dTO);
    }
}
