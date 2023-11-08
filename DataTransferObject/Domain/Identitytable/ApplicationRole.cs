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
    public class ApplicationRole : IdentityRole<int>
    {
        [NotMapped]
        public string? EncryptedId { get; set; }
        
        [NotMapped]
        [Display(Name = "S No.")]
        public int Sno { get; set; }
    }
}
