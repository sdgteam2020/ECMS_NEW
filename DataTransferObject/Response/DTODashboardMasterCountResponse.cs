using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTODashboardMasterCountResponse
    {
        public int TotUnit { get; set; }
        public int TotRank { get; set; }
        public int TotAppointment { get; set; }
        public int TotArms { get; set; }
        public int TotRegtCentre { get; set; }
    }
}
