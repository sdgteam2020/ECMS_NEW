using DataTransferObject.Domain.Model;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IAfsacCellMappingDB
    {
        public Task<List<DTOAfsacCellMappingResponse>?> GetAllAfsacCellMapping();
        public Task<bool> GetByName(AfsacCellMapping Dto);
    }
}
