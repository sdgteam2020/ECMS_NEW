using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOComdIdCheckInFKTableResponse
    {
        public byte TotalCorps { get; set; }
        public byte TotalBde { get; set; }
        public byte TotalDiv { get; set; }
        public int TotalMapUnit { get; set; }

    }
}
