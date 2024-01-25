using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Domain.Identitytable;

namespace DataTransferObject.Domain.Model
{
    public class TrnUnregdUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short UnregdUserId { get; set; }
        
        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; } = string.Empty;

        [StringLength(10)]
        [Column(TypeName = "varchar(10)")]
        public string ServiceNo { get; set; } = string.Empty;

        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string Rank { get; set; } = string.Empty;

        [StringLength(20)]
        [Column(TypeName = "varchar(20)")]
        public string DomainId { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        [StringLength(10)]
        [Column(TypeName = "varchar(10)")]
        public string? MobileNo { get; set; }


        [StringLength(6)]
        [Column(TypeName = "varchar(6)")]
        public string? DialingCode { get; set; }

        [StringLength(5)]
        [Column(TypeName = "varchar(5)")]
        public string? Extension { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
    }
}
