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
        public int TotInaccurateData { get; set; }
        public int TotObservationRaised { get; set; }
        public int TotLostCards { get; set; }
        public int TotHotlistCards { get; set; }
        public int TotDistCards { get; set; }
        public int TotMisprintedCard { get; set; }
        public int TotUnitChangeRequest { get; set; }
    }
}
