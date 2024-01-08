using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTODashboardCountResponse
    { 
        public int TotReq { get; set; }
        public int TotReject { get; set; }
        public int TotSelfPen { get; set; }
        public int TotIOPen { get; set; }
        public int TotGsoPen { get; set; }
        public int TotM11Pen { get; set; }
        public int TotGQ54Pen { get; set; }
        public int TotPrintPen { get; set; }
    }
}
