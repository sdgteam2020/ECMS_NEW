using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Master
{
    public class MBdeCat :Common
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BdeCatId { get; set; }
        [Required(ErrorMessage = "required!")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        public string BdeCatName { get; set; }

        public int ComdId { get; set; }
        
        public int CorpsId { get; set; }
        
    }
}
