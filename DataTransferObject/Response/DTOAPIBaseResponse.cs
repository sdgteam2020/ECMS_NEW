using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOAPIBaseResponse
    {
        public bool Status { get; set; } = false;
        public string Message { get; set; } = string.Empty;
    }
}
