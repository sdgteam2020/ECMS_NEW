using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOCardMovementHistoryResponse
    {
        public string StepName {  get; set; }
        public string ReportedBy {  get; set; }
        public string Remark {  get; set; }
        public DateTime ReportedOn {  get; set; }
    }
}
