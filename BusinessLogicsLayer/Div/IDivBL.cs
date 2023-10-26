using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Div
{
    public interface IDivBL : IGenericRepository<MDiv>
    {

        public Task<bool> GetByName(MDiv Data);
        public Task<List<DTODivResponse>> GetALLDiv(); 
        public Task<List<DTODivResponse>> GetByHId(DTOMHierarchyRequest Data);
    }
}

