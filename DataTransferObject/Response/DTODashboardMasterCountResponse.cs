using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTODashboardMasterCountResponse
    {
        public int TotComd { get; set; }
        public int TotCorps { get; set; }
        public int TotDiv { get; set; }
        public int TotBde { get; set; }
        public int TotMapUnit { get; set; }

        public int TotUnit { get; set; }
        public int TotRank { get; set; }
        public int TotAppointment { get; set; }
        public int TotArms { get; set; }
        public int TotRegtCentre { get; set; }

        public int TotDomainRegn { get; set; }
        public int TotUserProfile { get; set; }
    }
}
