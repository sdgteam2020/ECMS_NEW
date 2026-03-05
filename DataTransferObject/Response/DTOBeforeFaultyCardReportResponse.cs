using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOBeforeFaultyCardReportResponse
    {
        public int? TrnFaultyCardId { get; set; }
        public int RequestId { get; set; }
        public int? TrnFwdId { get; set; }
        public int BasicDetailId { get; set; }
        public byte ApplyForId { get; set; }
        public bool Result { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
