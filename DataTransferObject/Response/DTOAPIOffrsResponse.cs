using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOAPIOffrsResponse
    {
        public string Name { get; set; } = string.Empty;
        public string ServiceNo { get; set; } = string.Empty;
        public DateTime DOB { get; set; }
        public DateTime DateOfCommissioning { get; set; }
        public string PermanentAddress { get; set; } = string.Empty;
    }
}
