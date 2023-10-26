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
    public interface IMapUnitDB : IGenericRepositoryDL<MapUnit>
    {
        public Task<bool> GetByName(MapUnit Data);
        public Task<List<DTOMapUnitResponse>> GetALLUnit(DTOMHierarchyRequest unit);
    }
}
