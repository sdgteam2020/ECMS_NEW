using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IDomainMapDB : IGenericRepositoryDL<TrnDomainMapping>
    {
        public Task<TrnDomainMapping?> GetTrnDomainMappingByUserId(int UserId);
        public Task<bool> GetByDomainId(TrnDomainMapping Data);
        public Task<TrnDomainMapping> GetByDomainIdbyUnit(TrnDomainMapping Data);
        //public Task<TrnDomainMapping> GetByAspnetUserIdBy(TrnDomainMapping Data);
        public Task<TrnDomainMapping> GetByRequestId(int RequestId);
        public Task<TrnDomainMapping?> GetByAspnetUserIdBy(int AspNetUsersId);
        public Task<TrnDomainMapping?> GetAllRelatedDataByDomainId(string DomainId, string Role);
        public Task<TrnDomainMapping?> GetProfileDataByAspNetUserId(int Id);
    }
}
