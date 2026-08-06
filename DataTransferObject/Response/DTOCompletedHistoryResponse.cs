using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOCompletedHistoryResponse
    {
        public int RequestId { get; set; }
        public string ServiceNo { get; set; } = string.Empty;
        public string RankName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime CompletedOn { get; set; }
    }
}
