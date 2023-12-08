using DataTransferObject.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface ITrnFwnDB : IGenericRepositoryDL<MTrnFwd>
    {
        public Task<bool> UpdateAllBYRequestId(int RequestId);

    }
}
