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
    }
    public class DTOReportReturnCountlst
    {
        public List<DTOReportReturnCount> dTOReportReturnCount { get; set; }
        public List<DTOReportReturnCount> RecordOff { get; set; }
        public List<DTOReportReturnCount> RecordoffCount { get; set; }
    }
  }
