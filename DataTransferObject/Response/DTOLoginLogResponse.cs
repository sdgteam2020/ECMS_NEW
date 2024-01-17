using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{ 
    public class DTOLoginLogResponse
    { 
        public int Id { get; set; }
        public int AspNetUsersId { get; set; }
        public int UserId { get; set; }
        public string? IP { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set;}
        public string? DomainID { get; set;}
        public string? ArmyNo { get; set;}
        public string? Name { get; set;}
        public string? RankName { get; set;}
        public string? Updatedby { get; set;}
        public DateTime UpdatedOn { get; set;}

    }
}
