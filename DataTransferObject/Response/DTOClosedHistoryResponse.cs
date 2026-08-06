using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOClosedHistoryResponse
    {
        public int RequestId { get; set; }
        public string ServiceNo { get; set; } = string.Empty;
        public string RankName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime ClosedOn { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public string Authority { get; set; } = string.Empty;
    }
}
