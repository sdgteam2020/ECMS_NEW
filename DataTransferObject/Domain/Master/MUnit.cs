using DataTransferObject.Domain.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Master
{
    public class MUnit:Common
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UnitId { get; set; }
        [Required(ErrorMessage = "required!")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        [MaxLength(7)]
        [Column(TypeName = "varchar(7)")]
        public string Sus_no { get; set; } = string.Empty;

        [Required(ErrorMessage = "required!")]
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets.")]
        [MaxLength(1)]
        [Column(TypeName = "char(1)")]
        public string Suffix { get; set; } = string.Empty;

        [Required(ErrorMessage = "required!")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string UnitName { get; set; } = string.Empty;

      
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        [Column(TypeName = "varchar(10)")]
        [MaxLength(10)]
        public string? Abbreviation { get; set; } = string.Empty;

        [Required(ErrorMessage = "required!")]
        public bool IsVerify { get; set; }

        [ForeignKey("TrnUnregdUser")]
        public short? UnregdUserId { get; set; }
        public TrnUnregdUser? TrnUnregdUser { get; set; }
    }
}
