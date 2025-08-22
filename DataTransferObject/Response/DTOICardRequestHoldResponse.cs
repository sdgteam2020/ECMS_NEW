using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOICardRequestHoldResponse
    {
        public int TotalFilteredRecords { get; set; }
        public int ICardHoldId { get; set; }
        public int RequestId { get; set; }
        public string ServiceNo { get; set; } = string.Empty;
        public string RankName { get; set; } = string.Empty;
        public string FName { get; set; } = string.Empty;
        public string? LName { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public string ApplyFor { get; set; } = string.Empty;
        public string DomainId { get; set; } = string.Empty;
        public string HoldReason { get; set; } = string.Empty;
        public string? UnHoldReason { get; set; }
        public bool IsHold { get; set; }
        public DateTime UpdatedOn { get; set; }

    }
}
