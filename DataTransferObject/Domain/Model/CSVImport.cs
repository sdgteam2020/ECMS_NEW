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
    public class CSVImport
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        [Column(TypeName = "varchar(20)")]
        public string FileName { get; set; }

        [Required]
        public int TotalRecords { get; set; }
        [Required]
        public int ValidRecords { get; set; }

        [Required]
        public int DbInvalidRecords { get; set; }

        [Required]
        public int SheetInvalidRecords { get; set; }

        [Required]
        public bool DBUpdated { get; set; } = true;

        [ForeignKey("ApplicationUserUpdate"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? ImportedBy { get; set; }
        public ApplicationUser? ApplicationUserUpdate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "datetime")]
        public DateTime? ImportedOn { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
    }
}
