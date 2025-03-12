using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOUsersByClaim
    {
        public string DomainId { get; set; } = string.Empty;
        public string Rank { get; set; } = string.Empty;
        public string? ArmyNo { get; set; }
        public string AppointmentName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public List<string>? RoleNames { get; set; }
    }
}
