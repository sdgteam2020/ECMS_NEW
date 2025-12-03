using DataTransferObject.Domain.Identitytable;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTransferObject.Domain
{
    public class Common
    {
        [Required]
        public bool IsActive { get; set; }=true;
       
        [ForeignKey("ApplicationUserUpdate"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? Updatedby { get; set;}

        [JsonIgnore]
        public ApplicationUser? ApplicationUserUpdate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
    }
}
