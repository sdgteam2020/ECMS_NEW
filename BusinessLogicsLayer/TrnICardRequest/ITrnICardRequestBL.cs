using DataAccessLayer;
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
    public interface ITrnICardRequestBL : IGenericRepositoryDL<MTrnICardRequest>
    {
        public Task<MTrnICardRequest?> GetRequestByBasicDetailId(int BasicDetailId);
        public Task<MTrnICardRequest> GetByAspNetUserBy(int AspnetuserId);
        public Task<bool> GetRequestPendingUsingBasicDetailIds(int[] BasicDetailId);
        public Task<bool> GetRequestPending(int BasicDetailId);
        public Task<int> GetUserIdByRequestId(int RequestId);
        public Task<bool> UpdateStatus(int RequestId);

    }
}
