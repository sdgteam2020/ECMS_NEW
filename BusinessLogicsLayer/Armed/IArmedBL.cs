using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Master
{
    public interface IArmedBL : IGenericRepository<MArmedType>
    {

        public Task<bool> GetByName(MArmedType DTo);
        public Task<List<DTOArmedResponse>> GetALLArmed();
    }
}
