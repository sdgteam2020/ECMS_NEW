using DataTransferObject.Domain.Master;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTransferObject.Domain.Model
{
    public class TrnApplClose : Common
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [ForeignKey("MPostingReason"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public byte ReasonId { get; set; }
        public MPostingReason? MPostingReason { get; set; }
        
        [Required]
        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string Authority { get; set; }=string.Empty;

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string Remarks { get; set; } = string.Empty;


        [ForeignKey("MTrnICardRequest"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RequestId { get; set; }
        public MTrnICardRequest? MTrnICardRequest { get; set; }

        [ForeignKey("MUserProfile"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }
        public MUserProfile? MUserProfile { get; set; }

        [Column(TypeName = "varchar(max)")]
        public string CardRequestHistoryJson { get; set; }

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
