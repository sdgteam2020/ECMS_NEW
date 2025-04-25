using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.MapUnitChange
{
    public class MapUnitChangeBL:GenericRepositoryDL<TrnMapUnitChangeRequest>,IMapUnitChangeBL
    {
        private readonly IMapUnitChangeDB _UnitChangeDB;
        public MapUnitChangeBL(ApplicationDbContext context, IMapUnitChangeDB UnitChangeDB) : base(context)
        {
            _UnitChangeDB = UnitChangeDB;
        }
        public async Task<bool> FindUnitIdMapped(int UnitMapId) 
        {
            return await _UnitChangeDB.FindUnitIdMapped(UnitMapId);
        }
    }
}
