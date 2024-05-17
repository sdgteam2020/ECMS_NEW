using DataTransferObject.Domain.Model;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IOROMappingDB : IGenericRepositoryDL<OROMapping>
    {
        public Task<List<DTOOROMappingResponse>?> GetAllOROMapping();
        public Task<bool> GetByName(OROMapping Dto);
    }
}
