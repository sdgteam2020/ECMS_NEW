using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTransferObject.Domain.Master
{
    public class MBloodGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BloodGroupId { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string? BloodGroup { get; set; }
    }
}
