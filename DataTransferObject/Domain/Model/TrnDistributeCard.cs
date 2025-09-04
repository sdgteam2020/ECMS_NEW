using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTransferObject.Domain.Model
{
    public class TrnDistributeCard : Common
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DistributeCardId { get; set; }

        [ForeignKey("MTrnICardRequest"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RequestId { get; set; }
        public MTrnICardRequest? MTrnICardRequest { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "datetime")]
        public DateTime? DistributedOn { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string? Remark { get; set; }

        [ForeignKey("UserProfileUserUpdate"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? UpdatedbyUserId { get; set; }
        public MUserProfile? UserProfileUserUpdate { get; set; }
    }
}
