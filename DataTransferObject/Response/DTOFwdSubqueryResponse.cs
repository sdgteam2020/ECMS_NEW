using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOFwdSubqueryResponse
    {
        public string ServiceNo { get; set; } = string.Empty;
        public byte ArmedId { get; set; }
        public short Orderby { get; set; }
    }
}
