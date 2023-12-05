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
        public byte DivId { get; set; }

        [Required(ErrorMessage = "required!")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        public string DivName { get; set; }

        public byte ComdId { get; set; }
        public MComd? Comd { get; set; }
        public byte CorpsId { get; set; }
        public MCorps? Corps { get; set; }   




    }
}