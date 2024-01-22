using DataAccessLayer;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Unit
{
    public interface IMapUnitBL : IGenericRepository<MapUnit>
    {

        public Task<bool> GetByName(MapUnit Data);
        public Task<List<DTOMapUnitResponse>> GetALLUnit(DTOMHierarchyRequest unit,string Unit);
        public Task<List<DTOMapUnitResponse>> GetALLByUnitName(string Unitname);
        public Task<DTOMapUnitResponse> GetALLByUnitMapId(int UnitMapId); 
        public Task<DTOMapUnitResponse> GetALLByUnitById(int UnitId);
        public Task<bool?> SaveUnitWithMapping(DTOSaveUnitWithMappingByAdminRequest dTO);
    }
}

