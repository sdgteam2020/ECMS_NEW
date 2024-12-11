using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOVisitorCounterResponse
    {
        public int Today { get; set; }
        public int Week { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }=string.Empty;
        public int Total { get; set; }
    }
}
