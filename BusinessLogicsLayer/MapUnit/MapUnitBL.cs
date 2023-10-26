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

namespace BusinessLogicsLayer.Unit
{
    public class MapUnitBL : GenericRepositoryDL<MapUnit>, IMapUnitBL
    {
        private readonly IMapUnitDB _UnitDB;

        public MapUnitBL(ApplicationDbContext context, IMapUnitDB UnitDB) : base(context)
        {
            _UnitDB = UnitDB;
        }

        public Task<List<DTOMapUnitResponse>> GetALLUnit(DTOMHierarchyRequest unit)
        {
           return _UnitDB.GetALLUnit(unit);
        }

        public Task<bool> GetByName(MapUnit Data)
        {
            return _UnitDB.GetByName(Data); 
        }
    }
}
