using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Master
{
    public class MDiv : Common
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "DivId is number.")]
        public byte DivId { get; set; }

        [Required(ErrorMessage = "required!")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        [MaxLength(20, ErrorMessage = "Maximum length of Sus no is twenty character.")]
        [Column(TypeName = "varchar(20)")]
        public string DivName { get; set; } = string.Empty;

        [RegularExpression(@"^[\d]+$", ErrorMessage = "ComdId is number.")]
        public byte ComdId { get; set; }
        public MComd? Comd { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "CorpsId is number.")]
        public byte CorpsId { get; set; }
        public MCorps? Corps { get; set; }   




    }
}