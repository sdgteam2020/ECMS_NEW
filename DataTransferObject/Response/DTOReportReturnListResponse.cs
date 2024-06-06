using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOReportReturnListResponse
    {
        public int RequestId { get; set; }
        public int StepId { get; set; }
        public string? Name { get; set; }
        public string? ServiceNo { get; set; }
        public DateTime DOB { get; set; }
        public string? RankName { get; set; }
        public string? TrackingId { get; set; }
        public string? ArmyNoTo { get; set; }
        public string? NameTo { get; set; }
        public string? RankTo { get; set; }
        public string? ArmyNoFrom { get; set; }
        public string? NameFrom { get; set; }
        public string? RankFrom { get; set; }
        public string? DomainIdFrom { get; set; }
        public string? DomainIdTo { get; set; }
        public string? StatusName { get; set; } 

        public DateTime UpdatedOn { get; set; }

    }
}
