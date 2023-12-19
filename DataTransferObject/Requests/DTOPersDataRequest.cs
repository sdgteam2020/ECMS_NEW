using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOPersDataRequest
    {
        public string Pers_Army_No { get; set; }
        public string jwt { get; set; }
        public DateTime timestamp { get; set; }
        public string ClientName { get; set; }
    }
}
