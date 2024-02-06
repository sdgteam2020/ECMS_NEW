using DataTransferObject.Domain.Master;
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
    public interface IBdeBL : IGenericRepository<MBde>
    {

        public Task<bool> GetByName(MBde Data);
        public Task<List<DTOBdeResponse>> GetALLBdeCat();
        public Task<List<DTOBdeResponse>> GetByHId(DTOMHierarchyRequest Data);
        public Task<bool?> FindByBdeWithId(string BdeName, byte BdeId);


    }
}
