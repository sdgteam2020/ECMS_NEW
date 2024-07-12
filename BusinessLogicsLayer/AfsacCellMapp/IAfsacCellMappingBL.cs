using DataTransferObject.Domain.Model;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.AfsacCellMapp
{
    public interface IAfsacCellMappingBL : IGenericRepository<AfsacCellMapping>
    {
        public Task<List<DTOAfsacCellMappingResponse>?> GetAllAfsacCellMapping();
        public Task<bool> GetByName(AfsacCellMapping Dto);
    }
}
