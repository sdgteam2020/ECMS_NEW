using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOUnitResponse
    {
        public int UnitId { get; set; }
        public string Sus_no { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public string? Abbreviation { get; set; } = string.Empty;
        public bool IsVerify { get; set; }
    }
}
