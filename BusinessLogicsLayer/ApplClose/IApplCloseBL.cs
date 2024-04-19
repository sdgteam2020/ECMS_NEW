using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Posting
{
    public interface IApplCloseBL : IGenericRepository<TrnApplClose>
    {

        public Task<bool> RequestIdExists(TrnApplClose DTo);
    }
}
