using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataTransferObject.Response
{
    public class DTOLoginAPIResponse
    {
        //public int code { get; set; }
        //public bool error { get; set; }
        //public string msg { get; set; }
        //public int timestamp { get; set; }
        //public string jwt { get; set; }

        public string token { get; set; }
        public string expiration { get; set; }

    }
    public class DTOLoginAPIResponseData
    {
        public DTOLoginAPIResponse ValidateRequest { get; set; }
    }
}
