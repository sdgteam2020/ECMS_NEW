using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IBdeDB : IGenericRepositoryDL<MBde>
    {
        public Task<bool?> GetByName(MBde Data);
        public Task<List<DTOBdeResponse>> GetALLBdeCat();
        public Task<List<DTOBdeResponse>> GetByHId(DTOMHierarchyRequest Data);
        public Task<bool?> FindByBdeWithId(string BdeName, byte BdeId);
        public Task<DTOBdeIdCheckInFKTableResponse?> BdeIdCheckInFKTable(byte BdeId);
    }
}
