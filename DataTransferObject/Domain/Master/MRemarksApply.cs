using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Master
{
    public class MRemarksApply
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte RemarkApplyId { get; set; }
        [Required(ErrorMessage = "required!")]
        [Column(TypeName = "varchar(100)")]
        [MaxLength(100)]
        public string RemarksApply { get; set; } = string.Empty;
    }
}
