using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOTokenResponse
    {
        public bool IsToken { get; set; }
        public string Message { get; set; }
        public int MessageCode { get; set; }
        public string ArmyNo { get; set; }
    }
}
