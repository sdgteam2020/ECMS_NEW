using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOGetTaskCountICardRequest
    {
        public int UserId { get; set; }
        public int Type { get; set; }
        public int ApplyForId { get; set; }
        public int TDMId { get; set; }
        public int UnitId { get; set; }
        public byte CValue { get; set; }
    }
}
