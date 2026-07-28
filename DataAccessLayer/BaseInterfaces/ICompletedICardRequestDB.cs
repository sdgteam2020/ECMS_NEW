using DataTransferObject.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface ICompletedICardRequestDB
    {
        public Task<CompletedICardRequest?> GetByRequestId(int RequestId);
    }
}
