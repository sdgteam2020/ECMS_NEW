using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IDivDB : IGenericRepositoryDL<MDiv>
    {
        public Task<bool> GetByName(MDiv Data);
        public Task<List<DTODivResponse>> GetALLDiv();
        public Task<List<DTODivResponse>> GetByHId(DTOMHierarchyRequest Data);
    }
   
}
