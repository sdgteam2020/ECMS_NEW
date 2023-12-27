using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOTempSession
    {

        public int Status { get; set; }
        public string DomainId { get; set; } = string.Empty;
        public bool AdminFlag { get; set; } 
        public string RoleName { get; set; } = string.Empty;
        public string ICNO { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int TrnDomainMappingUnitId { get; set; }
        public int TrnDomainMappingId { get; set; }
        public int AspNetUsersId { get; set; }
        public string ICNOInput { get; set; } = string.Empty;
        public string? ICNoDomainId { get; set; }
        public int ICNoUserId { get; set; }
        public int ICNoTrnDomainMappingUnitId { get; set; }
        public int ICNoTrnDomainMappingId { get; set; }
        public string? ICNoUnitName { get; set; }

    }
}
