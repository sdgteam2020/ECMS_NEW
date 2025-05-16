using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOReportResponse
    {
        public int? RequestId { get; set; }
        public int? StepId { get; set; }
        public string? Name { get; set; }
        public string? FName { get; set; }
        public string? LName { get; set; }
        public string? ServiceNo { get; set; }
        public string? TrackingId { get; set; }
        public string? RankName { get; set; }
        public string? Status { get; set; }
        public string? ArmedAbbreviation { get; set; }
        public string? ApplyFor { get; set; }
    }
}
