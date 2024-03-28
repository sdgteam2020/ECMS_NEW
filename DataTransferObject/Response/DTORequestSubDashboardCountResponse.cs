using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTORequestSubDashboardCountResponse
    {
        public int TotDrafted { get; set; }
        public int TotSubmitted { get; set; }
        public int TotPrinted { get; set; }
        public int TotRaisedObsn { get; set; }
        public int TotRejected { get; set; }
        public int TotPostingOut { get; set; }
    }
}
