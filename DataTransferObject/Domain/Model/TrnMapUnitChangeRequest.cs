using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Master;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTransferObject.Domain.Model
{
    public class TrnMapUnitChangeRequest:Common
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MapUnitChangeRequestId { get; set; }

        [ForeignKey("MapUnit"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UnitMapId { get; set; }
        public MapUnit? MapUnit { get; set; }
       
        [Column(TypeName = "varchar(500)")]
        public string ExistingCh { get; set; } = string.Empty;

        [Column(TypeName = "varchar(100)")]
        public string RequestCh { get; set; } = string.Empty;

        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string Remark { get; set; } = string.Empty;

        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string? AdminRemark { get; set; }
        public bool IsComplete { get; set; } = false;
        
        [ForeignKey("MUserProfile"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FromUserId { get; set; }
        public MUserProfile? MUserProfile { get; set; }
        
        [ForeignKey("ApplicationUser"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? AdminUpdatedby { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        public DateTime? AdminUpdatedOn { get; set; }
        
        [ForeignKey("ApproverMUserProfile"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? AdminUserId { get; set; }
        public MUserProfile? ApproverMUserProfile { get; set; }
        public bool IsEditAction { get; set; }
        public bool RequestStatus { get; set; } = false;

    }
}
