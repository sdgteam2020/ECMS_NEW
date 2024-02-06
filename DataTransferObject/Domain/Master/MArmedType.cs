using DataTransferObject.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Master
{
    public class MArmedType:Common
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "ArmedId is number.")]
        public byte ArmedId { get; set; }
        
        [Required(ErrorMessage = "required!")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        [Column(TypeName = "varchar(100)")]
        [MaxLength(100, ErrorMessage = "Maximum length of Armed Name is One hundred character.")]
        public string ArmedName { get; set; } = string.Empty;

        [Required(ErrorMessage = "required!")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        [Column(TypeName = "varchar(10)")]
        [MaxLength(10, ErrorMessage = "Maximum length of Abbreviation is ten character.")]
        public string Abbreviation { get; set; } = string.Empty;

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool FlagInf { get; set; } = false;

        [ForeignKey("ArmedCat"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "ArmedCatId is number.")]
        public byte ArmedCatId { get; set; }
        public MArmedCat? ArmedCat { get; set; }
    }
}
