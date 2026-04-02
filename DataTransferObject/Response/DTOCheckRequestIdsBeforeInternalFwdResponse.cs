using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOCheckRequestIdsBeforeInternalFwdResponse
    {
        public int TrnFwdId { get; set; }
        public int ApplId { get; set; }
        public bool IsValid { get; set; } = false;
        public string Remarks { get; set; } = string.Empty;
    }
}
