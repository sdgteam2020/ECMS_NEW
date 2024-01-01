using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTORemarksResponse
    {
        public int RemarksId { get; set; }
        public string Remarks { get; set; }
        public string RemarksType { get; set; }
        public int RemarkTypeId { get; set; }
    }
}
