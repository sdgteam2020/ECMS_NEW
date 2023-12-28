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
        public byte ArmedId { get; set; }
        
        [Required(ErrorMessage = "required!")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        [Column(TypeName = "varchar(50)")]
        [MaxLength(50)]
        public string ArmedName { get; set; } = string.Empty;

        [Required(ErrorMessage = "required!")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        [Column(TypeName = "varchar(10)")]
        [MaxLength(10)]
        public string Abbreviation { get; set; } = string.Empty;
        public bool FlagInf { get; set; } = false;

        [ForeignKey("ArmedCat"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte ArmedCatId { get; set; }
        public MArmedCat? ArmedCat { get; set; }
    }
}
