using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Master
{
    public class MCorps:Common
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "CorpsId is number.")]
        public byte CorpsId { get; set; }

        [Required(ErrorMessage = "required!")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        [Column(TypeName = "varchar(10)")]
        [MaxLength(10, ErrorMessage = "Maximum length of Corps Name is ten character.")]
        public string CorpsName { get; set; } = string.Empty;
        [Required(ErrorMessage = "required!")]

        [ForeignKey("Comd"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "ComdId is number.")]
        public byte ComdId { get; set; }

        public MComd? Comd { get; set; }  
      
    }
}
