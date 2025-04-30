using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IMapUnitChangeDB:IGenericRepositoryDL<TrnMapUnitChangeRequest>
    {
        public Task<bool> FindUnitIdMapped(int UnitMapId);
        public Task<DTODataTablesResponse<DTOMapUnitChangeResponse>> GetAllMapUnitChange(DTODataTablesRequestForMapUnitChange request);
        public Task<DTOCommonSaveResponse> UpdateMapUnitChangeRequest(DTOSaveMapUnitChangeRequest dTO, TrnMapUnitChangeRequest trnMapUnit);
    }
}
