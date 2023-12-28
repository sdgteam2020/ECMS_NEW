using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace DataTransferObject.Domain.Master
{
    public class MRank:Common
    { 
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short RankId { get; set; }

        [Required(ErrorMessage = "required!")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        [Column(TypeName = "varchar(50)")]
        [MaxLength(50)]
        public string RankName { get; set; }

        [Required(ErrorMessage = "required!")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        [Column(TypeName = "varchar(30)")]
        [MaxLength(30)]
        public string RankAbbreviation { get; set; }

        public short Orderby { get; set; }
        [Required(ErrorMessage = "required!")]
        [ForeignKey("MApplyFor"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte ApplyForId { get; set; }
        public MApplyFor? MApplyFor { get; set; }

        public int? rank_cd { get; set; }
    }
}
