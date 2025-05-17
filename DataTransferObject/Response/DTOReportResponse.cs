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
        
        
        public string? UnitAbbreviation { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string RemarksIds { get; set; } = string.Empty;
        public string? FromRemark { get; set; }
        public string? ToRemark { get; set; }
    }
}
