using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOReportDashboardCountResponse
    {
        public int TotRequisition { get; set; }
        public int TotLostCases { get; set; }
        public int TotMonthlyProcessed { get; set; }
        public int TotNonFunctionalCard { get; set; }

    }
}
