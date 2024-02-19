using BusinessLogicsLayer.Bde;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer;
using DataTransferObject.Domain.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Response;
using DataTransferObject.Requests;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusinessLogicsLayer.Unit
{
    public class MapUnitBL : GenericRepositoryDL<MapUnit>, IMapUnitBL
    {
        private readonly IMapUnitDB _UnitDB;

        public MapUnitBL(ApplicationDbContext context, IMapUnitDB UnitDB) : base(context)
        {
            _UnitDB = UnitDB;
        }

        public Task<DTOMapUnitResponse> GetALLByUnitById(int UnitId)
        {
            return _UnitDB.GetALLByUnitById(UnitId);
        }

        public Task<DTOMapUnitResponse> GetALLByUnitMapId(int UnitMapId)
        {
            return _UnitDB.GetALLByUnitMapId(UnitMapId);
        }

        public Task<List<DTOMapUnitResponse>> GetALLByUnitName(string Unitname)
        {
            return _UnitDB.GetALLByUnitName(Unitname);
        }

        public Task<List<DTOMapUnitResponse>> GetALLUnit(DTOMHierarchyRequest unit,string Unit1)
        {
           return _UnitDB.GetALLUnit(unit, Unit1);
        }

        public Task<bool> GetByName(MapUnit Data)
        {
            return _UnitDB.GetByName(Data); 
        }
        public async Task<bool?> SaveUnitWithMapping(DTOSaveUnitWithMappingByAdminRequest dTO)
        {
            return await _UnitDB.SaveUnitWithMapping(dTO);
        }
        public async Task<bool?> FindUnitId(int UnitId)
        {
            return await _UnitDB.FindUnitId(UnitId);
        }
        public async Task<bool?> FindUnitIdMapped(int UnitId, int UnitMapId)
        {
            return await _UnitDB.FindUnitIdMapped(UnitId, UnitMapId);
        }

    }
}
