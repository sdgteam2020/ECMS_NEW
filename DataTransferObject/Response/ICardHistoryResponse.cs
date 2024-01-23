using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{ 
    public class ICardHistoryResponse
    {
        public string? FromDomain { get; set; }
        public string? FromProfile { get; set; }
        public string? FromRank { get; set; }
        public string? ToDomain { get; set; }
        public string? ToProfile { get; set; }
        public string? ToRank { get; set; }
        public string? Status { get; set; }
        public string? UpdatedOn { get; set; }
        public string? Remark { get; set; }
        public int IsComplete { get; set; }
        public string? Remarks2 { get; set; }
        public string? Reason { get; set; }
        public string? Authority { get; set; }
        public string? UnitName { get; set; }
    }
}
