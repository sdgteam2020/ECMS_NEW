using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Bde
{
    public interface ITrnICardRequestBL : IGenericRepository<MTrnICardRequest>
    {
        public Task<MTrnICardRequest> GetByAspNetUserBy(int AspnetuserId);
        public Task<bool> GetRequestPending(int BasicDetailId);
        public Task<int> GetUserIdByRequestId(int RequestId);
        public Task<bool> UpdateStatus(int RequestId);

    }
}
