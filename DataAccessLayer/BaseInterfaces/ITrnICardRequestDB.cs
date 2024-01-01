using DataTransferObject.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface ITrnICardRequestDB : IGenericRepositoryDL<MTrnICardRequest>
    {
        public Task<MTrnICardRequest> GetByAspNetUserBy(int AspnetuserId);
        public Task<bool> GetRequestPending(int BasicDetailId);
        public Task<int> GetUserIdByRequestId(int RequestId);

    }
}
