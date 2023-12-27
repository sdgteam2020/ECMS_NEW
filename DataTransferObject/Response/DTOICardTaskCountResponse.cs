using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{ 
    public class DTOICardTaskCountResponse
    {
        public int IOPending { get; set; }
        public int IOApproved { get; set; }
        public int IOReject { get; set; }
        public int GSOPending { get; set; }
        public int GSOApproved { get; set; }
        public int GSOReject { get; set; }
        public int MIPending { get; set; }
        public int MIApproved { get; set; }
        public int MIReject { get; set; }
        public int HQPending { get; set; }
        public int HQApproved { get; set; }
        public int HQReject { get; set; }
       
    }
}
