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
        [RegularExpression(@"^[\d]+$", ErrorMessage = "UnitId is number.")]
        public int UnitId { get; set; }
        
        [Required(ErrorMessage = "required!")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "Numbers allowed.")]
        [MaxLength(7, ErrorMessage = "Maximum length of Sus no is seven digit.")]
        [Column(TypeName = "varchar(7)")]
        public string Sus_no { get; set; } = string.Empty;

        [Required(ErrorMessage = "required!")]
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets.")]
        [MaxLength(1,ErrorMessage = "Maximum length of Suffix is one character.")]
        [MinLength(1, ErrorMessage = "Minimum length of Suffix is one character.")]
        [Column(TypeName = "char(1)")]
        public string Suffix { get; set; } = string.Empty;

        //[Required(ErrorMessage = "required!")]
        //[RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        //[MaxLength(100, ErrorMessage = "Maximum length of UnitName is 100 character.")]
        //[Column(TypeName = "varbinary(Max)")]
        //public Byte[]? UnitDesc { get; set; }


        [Required(ErrorMessage = "required!")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        [MaxLength(100, ErrorMessage = "Maximum length of UnitName is 100 character.")]
        [Column(TypeName = "varchar(100)")]
        public string UnitName { get; set; } = string.Empty;

        [Column(TypeName = "varchar(10)")]
        [MaxLength(10, ErrorMessage = "Maximum length of Abbreviation is ten character.")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        public string Abbreviation { get; set; } = string.Empty;

        [Required(ErrorMessage = "required!")]
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool IsVerify { get; set; }

        [ForeignKey("TrnUnregdUser")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "UnregdUserId is number.")]
        public short? UnregdUserId { get; set; }
        public TrnUnregdUser? TrnUnregdUser { get; set; }
    }
}
