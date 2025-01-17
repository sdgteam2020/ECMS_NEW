using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOCheckUnitMappedInMapUnitResponse
    {
        public int? UnitId { get; set; }
        public bool IsVerify { get; set; }
        public int? UnitMapId { get; set; }
    }
}
