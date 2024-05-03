using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOUnitMapIdCheckInFKTableResponse
    {
        public int TotalBD { get; set; }
        public int TotalRO { get; set; }
        public int TotalTDM { get; set; }
        public int TotalTF { get; set; }
        public int TotalTPOFrom { get; set; }
        public int TotalTPOTo { get; set; }
    }
}
