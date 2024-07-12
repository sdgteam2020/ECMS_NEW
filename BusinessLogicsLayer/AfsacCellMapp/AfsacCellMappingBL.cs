using DataAccessLayer.BaseInterfaces;
using DataAccessLayer;
using DataTransferObject.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
