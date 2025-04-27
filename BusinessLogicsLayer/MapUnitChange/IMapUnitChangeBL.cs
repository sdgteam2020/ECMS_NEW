using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
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
        public Task<DTODataTablesResponse<DTOMapUnitChangeResponse>> GetAllMapUnitChange(DTODataTablesRequestForMapUnitChange request);
    }
}
