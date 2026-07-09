using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTransferObject.Domain.Model
{
    public class CompletedICardRequest : Common
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CompletedId { get; set; }

        [ForeignKey("MTrnICardRequest"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RequestId { get; set; }
        public MTrnICardRequest? MTrnICardRequest { get; set; }

        [Column(TypeName = "varchar(max)")]
        public string CardRequestHistoryJson { get; set; }

        [ForeignKey("UserProfileUserUpdate"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? UpdatedbyUserId { get; set; }
        public MUserProfile? UserProfileUserUpdate { get; set; }

        [Column(TypeName = "varchar(30)")]
        [MaxLength(30, ErrorMessage = "Maximum length of Rank Abbreviation is thirty character.")]
        public string RankAbbreviation { get; set; } = string.Empty;

        [StringLength(36)]
        [Column(TypeName = "varchar(36)")]
        public string Name { get; set; } = string.Empty;

        [StringLength(10)]
        [Column(TypeName = "varchar(10)")]
        public string ServiceNo { get; set; } = string.Empty;
    }
}
