using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Model
{
    public class TrnLostCard : Common
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LostCardId { get; set; }

        [ForeignKey("MTrnICardRequest"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RequestId { get; set; }
        public MTrnICardRequest? MTrnICardRequest { get; set; }

        [StringLength(250)]
        [Column(TypeName = "varchar(100)")]
        public string? Remark { get; set; }

        [Required]
        public bool IsFIRLogged { get; set; } = true;

        [StringLength(100)]
        [Column(TypeName = "VARCHAR(100)")]
        public string SupportDocName { get; set; } = string.Empty;

        [Column(TypeName = "NVARCHAR(MAX)")]
        public string SignedXML { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "datetime")]
        public DateTime? LostOn { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
    }
}
