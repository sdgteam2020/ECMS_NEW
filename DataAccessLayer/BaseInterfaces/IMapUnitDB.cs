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
        public Task<List<DTOMapUnitResponse>> GetALLUnit(DTOMHierarchyRequest unit,string Unit1);
        public Task<List<DTOMapUnitResponse>> GetALLByUnitName(string Unitname);
        public Task<DTOMapUnitResponse> GetALLByUnitMapId(int UnitMapId);
        public Task<DTOMapUnitResponse> GetALLByUnitById(int UnitId);
        public Task<bool?> SaveUnitWithMapping(DTOSaveUnitWithMappingByAdminRequest dTO);
        public Task<bool?> FindUnitId(int UnitId);
        public Task<bool?> FindUnitIdMapped(int UnitId, int UnitMapId);
    }
}
