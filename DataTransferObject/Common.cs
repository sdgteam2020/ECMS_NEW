using DataTransferObject.Domain.Identitytable;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain
{
    public class Common
    {
        [Required]
        public bool IsActive { get; set; }=true;
       
        [ForeignKey("ApplicationUserUpdate"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? Updatedby { get; set;}
        public ApplicationUser? ApplicationUserUpdate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
    }
}
