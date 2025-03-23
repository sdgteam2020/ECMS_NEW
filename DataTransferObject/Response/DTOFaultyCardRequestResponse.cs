using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOFaultyCardRequestResponse
    {
        public int RequestId { get; set; }
        public DateTime RequestDate { get; set; }
        public string FName { get; set; }=string.Empty;
        public string? LName { get; set; }
        public string ServiceNo { get; set; } = string.Empty;
        public string RankName { get; set; } = string.Empty;
        public string ApplyFor { get; set; } = string.Empty;
        public string PhotoImagePath { get; set; } = string.Empty;
    }
}
