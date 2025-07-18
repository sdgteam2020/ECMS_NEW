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
        public int SheetInValidRecords { get; set; }
        public string File { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
    }
}
