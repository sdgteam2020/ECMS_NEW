using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTransferObject.Domain.Master
{
    public class MDispatchMode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte DispatchModeId { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR(50)")]
        public string Description { get; set; } = string.Empty;
    }
}
