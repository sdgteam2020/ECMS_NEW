using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IICardHoldDB : IGenericRepositoryDL<MTrnICardHold>
    {
        public Task<bool> GetByRequestId(MTrnICardHold dTO);
        public Task<DTOBeforeSaveICardRequestHoldResponse> CheckBeforeICardRequestHold(MTrnICardHold hold);
    }
}
