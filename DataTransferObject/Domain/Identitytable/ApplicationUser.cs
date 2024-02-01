using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Identitytable
{
    public class ApplicationUser : IdentityUser<int>
    {
        [StringLength(20)]
        [Column(TypeName = "varchar(20)")]
        public string DomainId { get; set; } = string.Empty;
        public bool Active { get; set; } = false;
        public bool AdminFlag { get; set; } = false;
        public DateTime? AdminFlagDate { get; set; }

        [Display(Name = "Updated By")]
        public int Updatedby { get; set; }

        [Display(Name = "Updated On")]
        [DataType(DataType.Date)]
        [Column(TypeName = "datetime")]
        public DateTime UpdatedOn { get; set; }
        public string? AdminMsg { get; set; }

        public string? Fd1 { get; set; }
        public string? Fd2 { get; set;}
    }
}