using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTransferObject.Domain.Master
{
    public class MPostingReason
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte Id { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string Reason { get; set; } = string.Empty;

        public int Type { get; set; }
    }
}
