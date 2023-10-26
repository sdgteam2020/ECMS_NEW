using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.BdeCat
{
    public interface IBdeCatBL : IGenericRepository<MBdeCat>
    {

        public Task<bool> GetByName(MBdeCat Data);
        public Task<List<DTOBdeCatResponse>> GetALLBdeCat();
    }
}
