using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTransferObject.Domain.Model
{
    public class MTrnICardHold:Common
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ICardHoldId { get; set; }
        
        [Column(TypeName = "varchar(50)")]
        public string HoldReason { get; set; } = string.Empty;

        [Column(TypeName = "varchar(50)")]
        public string? UnHoldReason { get; set; }

        [ForeignKey("MTrnICardRequest"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RequestId { get; set; }
        public MTrnICardRequest? MTrnICardRequest { get; set; }

        [ForeignKey("MUserProfile"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }
        public MUserProfile? MUserProfile { get; set; }
        public bool IsHold { get; set; }
    }
}
