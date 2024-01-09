using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTODomainRegnRequest
    {
        public int Id { get; set; }
        public string DomainId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public bool AdminFlag { get; set; }
        public bool Active { get; set; }
        public int TDMId { get; set; }
        public int UnitMappId { get; set; }
        public short ApptId { get; set; }
        public int UserId { get; set; }
        public string? ArmyNo { get; set; }
    }
}
