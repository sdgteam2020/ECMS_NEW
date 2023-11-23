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
        public int BdeId { get; set; }
        [Required(ErrorMessage = "required!")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        public string BdeName { get; set; }


        [ForeignKey("Comd")]
        public int ComdId { get; set; }

        public MComd? Comd { get; set; }

        [ForeignKey("MCorps")]
        public int CorpsId { get; set; }
        public MCorps? Corps { get; set; }

        [ForeignKey("MDiv")]

        [Required(ErrorMessage = "required!")]

        public int DivId { get; set; }
        public MDiv? Div { get; set; }   

    }
}
