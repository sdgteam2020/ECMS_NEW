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
        public Task<bool> GetByDomainId(TrnDomainMapping Data);
        public Task<TrnDomainMapping?> GetByDomainIdbyUnit(TrnDomainMapping Data);
        public Task<TrnDomainMapping?> GetByAspnetUserIdBy(TrnDomainMapping Data);
    }
}
