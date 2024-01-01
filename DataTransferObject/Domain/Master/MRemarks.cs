using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Master
{
    public class MRemarks
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte RemarksId { get; set; }
        [Required(ErrorMessage = "required!")]
        [Column(TypeName = "varchar(500)")]
        [MaxLength(500)]
        public string Remarks{ get; set; } = string.Empty;

        [ForeignKey("MRemarkType"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte RemarkTypeId { get; set; }


        public MRemarkType? MRemarkType { get; set; }

        [ForeignKey("MRemarksApply"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte RemarkApplyId { get; set; }


        public MRemarksApply? MRemarksApply { get; set; }

    }
}
