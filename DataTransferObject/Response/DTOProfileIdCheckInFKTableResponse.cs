using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOProfileIdCheckInFKTableResponse
    {
        public int TotalTDM { get; set; }
        public int TotalTH { get; set; }
        public int TotalTPO_To { get; set; }
        public int TotalTPO_From { get; set; }
        public int TotalTFFrom { get; set; }
        public int TotalTFTo { get; set; }
    }
}
