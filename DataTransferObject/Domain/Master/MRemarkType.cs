using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
