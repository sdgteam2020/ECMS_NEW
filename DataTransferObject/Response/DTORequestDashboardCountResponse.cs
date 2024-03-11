using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTORequestDashboardCountResponse
    {
        public int ToDraftedOffrs { get; set; }
        public int ToDraftedJCO { get; set; }
        public int ToSubmittedOffrs { get; set; }
        public int ToSubmittedJCO { get; set; }
        public int ToPostingOutJCO { get; set; }
        public int ToCourseJCO { get; set; }
        public int ToObsnRaisedOASIS { get; set; }
        public int ToObsnRaisedINDRA { get; set; }
        public int ToHotlistedICard { get; set; }
        public int ToBlockExistingICard { get; set; }
        public int ToDepositICard { get; set; }
    }
}
