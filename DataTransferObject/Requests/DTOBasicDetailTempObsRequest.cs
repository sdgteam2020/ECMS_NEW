using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOBasicDetailTempObsRequest
    {
        public int TDMId { get; set; }
        public int AspNetUsersId { get; set; }
        public string Name { get; set; } = string.Empty;
        public byte ArmedId { get; set; }
    }
}
