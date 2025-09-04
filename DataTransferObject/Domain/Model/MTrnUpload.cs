using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTransferObject.Domain.Model
{ 
    public class MTrnUpload
    { 
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UploadId { get; set; }
        [ForeignKey("BasicDetail"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BasicDetailId { get; set; }
        public BasicDetail? BasicDetail { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR(100)")]
        public string SignatureImagePath { get; set; } = string.Empty;

        [StringLength(100)]
        [Column(TypeName = "VARCHAR(100)")]
        public string PhotoImagePath { get; set; } = string.Empty;
    }
}
