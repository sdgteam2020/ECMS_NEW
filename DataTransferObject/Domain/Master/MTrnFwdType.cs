using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTransferObject.Domain.Master
{
    public class MTrnFwdType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte TypeId { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; }=string.Empty;
    }
}
