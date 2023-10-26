using DataTransferObject.Domain.Master;
using DataTransferObject.Response.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Master
{
    public interface IComd : IGenericRepository<DataTransferObject.Domain.Master.Comd>
    {

        public Task<bool> GetByName(DataTransferObject.Domain.Master.Comd DTo);
    }
}
