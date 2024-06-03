using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOReportReturnCount
    {
        public int StepId { get; set; }
        public int Total { get; set; }
        public string? Name { get; set; }
        public string? RecordOfficeId { get; set; }
        public int TypeId { get; set; }
        public int IsComplete { get; set; }
        public int IsApprove { get; set; }
    }
    public class DTOReportReturnCountlst
    {
        public List<DTOReportReturnCount> dTOReportReturnCountOffs { get; set; }

        public List<DTOReportReturnCount> dToCountApprovedRejectOffs { get; set; }

        public List<DTOReportReturnCount> dTOReportReturnCountJco { get; set; }
        public List<DTOReportReturnCount> dToCountApprovedRejectJco { get; set; }

        public List<DTOReportReturnCount> RecordOff { get; set; }
        public List<DTOReportReturnCount> RecordoffCount { get; set; }
        public List<DTOReportReturnCount> RecordJco { get; set; }
        public List<DTOReportReturnCount> RecordJcoPending { get; set; }
        public List<DTOReportReturnCount> RecordJcoCountApproved { get; set; }
    }
  }
