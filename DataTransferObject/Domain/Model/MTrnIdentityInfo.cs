using DataTransferObject.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTransferObject.Domain.Model
{
    public class MTrnIdentityInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InfoId { get; set; }
        [ForeignKey("BasicDetail"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BasicDetailId { get; set; }
        public BasicDetail? BasicDetail { get; set; }

        [StringLength(200)]
        [Column(TypeName = "VARCHAR(200)")]
        public string IdenMark1 { get; set; } = string.Empty;
        [StringLength(200)]
        [Column(TypeName = "VARCHAR(200)")]
        public string? IdenMark2 { get; set; } = string.Empty;

        [MaxLength(12)]
        [MinLength(12)]
        public long AadhaarNo { get; set; }
        [MinLength(ConstantsMinMax.IntMinLength)]
        public float Height { get; set; }

        public byte BloodGroupId { get; set; }
    }
}
