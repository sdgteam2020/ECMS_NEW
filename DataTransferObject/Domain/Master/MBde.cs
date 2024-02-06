using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Master
{
    public class MBde :Common
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "BdeId is number.")]
        public byte BdeId { get; set; }
        
        [Required(ErrorMessage = "required!")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        [Column(TypeName = "varchar(20)")]
        [MaxLength(20, ErrorMessage = "Maximum length of Bde Name is twenty character.")]
        public string BdeName { get; set; } = string.Empty;


        [ForeignKey("Comd")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "ComdId is number.")]
        public byte ComdId { get; set; }

        public MComd? Comd { get; set; }

        [ForeignKey("MCorps")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "CorpsId is number.")]
        public byte CorpsId { get; set; }
        public MCorps? Corps { get; set; }

        [ForeignKey("MDiv")]

        [Required(ErrorMessage = "required!")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "DivId is number.")]
        public byte DivId { get; set; }
        public MDiv? Div { get; set; }   

    }
}
