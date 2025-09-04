using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTransferObject.Domain.Model
{
    public class MTrnNotificationDisplay
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int DisplayId { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR(100)")]
        [Required]
        public string Spanname { get; set; }

        [StringLength(100)]
        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string Message { get; set; }
        [StringLength(200)]
     
        [Column(TypeName = "VARCHAR(200)")]
        public string Url { get; set; }
    }
}
