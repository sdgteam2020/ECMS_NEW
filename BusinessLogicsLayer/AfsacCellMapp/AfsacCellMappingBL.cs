using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.AfsacCellMapp
{
    public class AfsacCellMappingBL : GenericRepositoryDL<AfsacCellMapping>, IAfsacCellMappingBL
    {
        private readonly IAfsacCellMappingDB _AfsacCellMappingDB;
        public AfsacCellMappingBL(ApplicationDbContext context, IAfsacCellMappingDB iAfsacCellMappingDB) : base(context)
        {
            _AfsacCellMappingDB = iAfsacCellMappingDB;
        }
        public async Task<bool> GetByName(AfsacCellMapping Dto)
        {
            return await _AfsacCellMappingDB.GetByName(Dto);
        }
        public async Task<List<DTOAfsacCellMappingResponse>?> GetAllAfsacCellMapping()
        {
            return await _AfsacCellMappingDB.GetAllAfsacCellMapping();
        }
        public async Task<DTODataTablesResponse<DTOAfsacCellMappingResponse>> GetAllAfsacCellMapping_Pagination(DTODataTablesRequest dTO)
        {
            return await _AfsacCellMappingDB.GetAllAfsacCellMapping_Pagination(dTO);
        }
    }
}
