using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOCardDispatchCheckResponse
    {
        public int TotalRecords { get; set; }
        public int ValidRecords { get; set; }
        public int DbInValidRecords { get; set; }
        public int LotNo { get; set; }
        public string FileName { get; set; } = string.Empty;
    }
}
