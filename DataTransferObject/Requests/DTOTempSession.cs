using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
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
        public string Name { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int TDMUnitMapId { get; set; }
        public int TDMId { get; set; }
        public short TDMApptId { get; set; }
        public int AspNetUsersId { get; set; }
        public string ICNOInput { get; set; } = string.Empty;
        public string? ICNoDomainId { get; set; }
        public int ICNoUserId { get; set; }
        public int ICNoTDMUnitMapId { get; set; }
        public int ICNoTDMId { get; set; }
        public short ICNoTDMApptId { get; set; }
       
       
    }
}
