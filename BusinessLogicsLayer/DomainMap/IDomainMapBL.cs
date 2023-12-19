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
    public interface IDomainMapBL : IGenericRepository<TrnDomainMapping>
    {
        public Task<TrnDomainMapping?> GetTrnDomainMappingByUserId(int UserId);
        public Task<bool> GetByDomainId(TrnDomainMapping Data);
        public Task<TrnDomainMapping> GetByDomainIdbyUnit(TrnDomainMapping Data);
        //public Task<TrnDomainMapping> GetByAspnetUserIdBy(TrnDomainMapping Data);
        public Task<TrnDomainMapping> GetByRequestId(int RequestId);
        public Task<TrnDomainMapping?> GetByAspnetUserIdBy(int AspNetUsersId);

    }
}
