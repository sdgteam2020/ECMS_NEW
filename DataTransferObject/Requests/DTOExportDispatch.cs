using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOExportDispatch
    {
        public bool Allstatus { get; set; }
        public int[]? checkedRequestId { get; set; }
        public int[]? unchedRequestId { get; set; }
    }
}
