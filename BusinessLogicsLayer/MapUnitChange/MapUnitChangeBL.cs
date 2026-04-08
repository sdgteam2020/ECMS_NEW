using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
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
        public async Task<DTODataTablesResponse<DTOMapUnitChangeResponse>> GetAllMapUnitChange(DTODataTablesRequestForMapUnitChange request)
        {
            return await _UnitChangeDB.GetAllMapUnitChange(request);
        }
        public async Task<DTOCommonSaveResponse> UpdateMapUnitChangeRequest(DTOSaveMapUnitChangeRequest dTO, TrnMapUnitChangeRequest trnMapUnit)
        {
            if (dTO.Choice == 3)
            {
                trnMapUnit.RequestStatus = false;
            }
            else
            {
                trnMapUnit.RequestStatus = true;
            }
            return await _UnitChangeDB.UpdateMapUnitChangeRequest(dTO, trnMapUnit);
        }
        public async Task<DTOMapUnitDetailsResponse> GetUnitMoveHistory(DTOMapUnitDetailsResponse dTO)
        {
            return await _UnitChangeDB.GetUnitMoveHistory(dTO);
        }
    }
}
