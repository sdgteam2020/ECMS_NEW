using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTORegisterUserResponse
    {
        public string DomainId { get; set; } = string.Empty;
        public string? ArmyNo { get; set; }
        public string? Rank { get; set; } 
        public string? Name { get; set; }
        public string AppointmentName { get; set; } = string.Empty;
    }
}
