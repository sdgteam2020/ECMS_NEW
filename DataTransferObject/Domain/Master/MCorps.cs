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
        public int CorpsId { get; set; }

        [Required(ErrorMessage = "required!")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        public string CorpsName { get; set; }
        [Required(ErrorMessage = "required!")]

        [ForeignKey("Comd"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ComdId { get; set; }

        
        public Comd? Comd { get; set; }  
      
    }
}
