using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DtoSession
    {
        public string ICNO { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string RankName { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int UnitId { get; set; }
        public int TrnDomainMappingId { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public Boolean IsToken { get; set; }
        // DoaminId Stored in session for IAM
        public string DoaminId { get; set; } = string.Empty;
    }
}
