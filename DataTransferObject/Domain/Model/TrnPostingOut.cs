using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Master;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTransferObject.Domain.Model
{
    public class TrnPostingOut:Common
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
        public string Authority { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "datetime")]
        public DateTime SOSDate { get; set; }
        
        [ForeignKey("ApplicationUser"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FromAspNetUsersId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

        [ForeignKey("MapUnit"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FromUnitID { get; set; }
        public MapUnit? MapUnit { get; set; }
        [ForeignKey("MUserProfile"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FromUserID { get; set; }
        public MUserProfile? MUserProfile { get; set; }
        [ForeignKey("ToApplicationUser"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ToAspNetUsersId { get; set; }
        public ApplicationUser? ToApplicationUser { get; set; }
        [ForeignKey("ToMapUnit"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ToUnitID { get; set; }
        public MapUnit? ToMapUnit { get; set; }
        [ForeignKey("ToMUserProfile"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ToUserID { get; set; }
        
        public MUserProfile? ToMUserProfile { get; set; }

        [ForeignKey("MTrnICardRequest"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RequestId { get; set; }
        public MTrnICardRequest? MTrnICardRequest { get; set; }

        [ForeignKey("MTrnFwd"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? TrnFwdId { get; set; }
        public MTrnFwd? MTrnFwd { get; set; }
        [DataType(DataType.Date)]
        [Column(TypeName = "datetime")]
        public DateTime? DispatchedOn { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar(20)")]
        public string? RefNo { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "datetime")]
        public DateTime? DispatchUpdatedOn { get; set; }

        [ForeignKey("DispatchUserUpdate"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? DispatchUpdatedBy { get; set; }
        public ApplicationUser? DispatchUserUpdate { get; set; }
    }
}
