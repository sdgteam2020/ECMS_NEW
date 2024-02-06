using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Master
{
    public class MComd: Common
    { 
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "ComdId is number.")]
        public byte ComdId { get; set; }

        [Required(ErrorMessage = "required!")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        [Column(TypeName = "varchar(50)")]
        [MaxLength(50, ErrorMessage = "Maximum length of Comd Name is fifty character.")]
        public string ComdName { get; set; } = string.Empty;
        [Required(ErrorMessage = "required!")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        [Column(TypeName = "varchar(20)")]
        [MaxLength(20, ErrorMessage = "Maximum length of Comd Abbreviation is twenty character.")]
        public string ComdAbbreviation { get; set; } = string.Empty;

        [Required(ErrorMessage = "required!")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "Orderby is number.")]
        public int Orderby { get; set; }
    }
}
