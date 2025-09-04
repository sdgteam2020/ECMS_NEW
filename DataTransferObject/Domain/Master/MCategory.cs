using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTransferObject.Domain.Master
{
    public class MCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte CategoryId { get; set; }
        
        [StringLength(30)]
        [Column(TypeName = "varchar(30)")]
        public string Name { get; set; } = string.Empty;
    }
}
