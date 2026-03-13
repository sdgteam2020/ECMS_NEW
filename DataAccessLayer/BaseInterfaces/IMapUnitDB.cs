using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IMapUnitDB : IGenericRepositoryDL<MapUnit>
    {
        public Task<DTOGenericResponse<DTOCheckUnitMappedInMapUnitResponse>> CheckUnitMappedInMapUnit(string SUSNo);
        public Task<bool> GetByName(MapUnit Data);
        public Task<DTODataTablesResponse<DTOMapUnitResponse>> GetALLUnit(DTODataTablesRequestForMapUnit request);
        public Task<List<DTOMapUnitResponse>> GetALLByUnitName(string Unitname);
        public Task<DTOMapUnitResponse> GetALLByUnitMapId(int UnitMapId);
        public Task<DTOMapUnitResponse> GetALLByUnitById(int UnitId);
        public Task<bool?> SaveUnitWithMapping(DTOSaveUnitWithMappingByAdminRequest dTO);
        public Task<bool?> FindUnitId(int UnitId);
        public Task<bool?> FindUnitIdMapped(int UnitId, int UnitMapId);
        public Task<DTOUnitMapIdCheckInFKTableResponse?> UnitMapIdCheckInFKTable(int UnitMapId);
        public Task<List<DTOUnitResponse>> GetUnitByHierarchy(DTOMHierarchyRequest Data);
        public Task<List<DTOUnitResponse>> GetUnitByHierarchyForIcardRequest(DTOMHierarchyRequest Data);


    }
}
