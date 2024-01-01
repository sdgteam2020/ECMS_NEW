using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Master
{
    public class MRemarkType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte RemarkTypeId { get; set; }
        [Required(ErrorMessage = "required!")]
        [Column(TypeName = "varchar(500)")]
        [MaxLength(500)]
        public string RemarksType { get; set; }=string.Empty;
    }
}
