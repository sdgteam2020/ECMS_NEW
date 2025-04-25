using DataTransferObject.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.MapUnitChange
{
    public interface IMapUnitChangeBL : IGenericRepository<TrnMapUnitChangeRequest>
    {
        public Task<bool> FindUnitIdMapped(int UnitMapId);
    }
}
