using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOCheckBeforeDestructionCardReportResponse
    {
        public bool Result { get; set; }
        public string Message { get; set; } = string.Empty;
        public byte StatusId { get; set; }
        public int? ApplCloseId { get; set; }
        public int? CompletedId { get; set; }
    }
}
