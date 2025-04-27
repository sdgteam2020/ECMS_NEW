using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response.User;
using DataTransferObject.Domain.Identitytable;

namespace DataTransferObject.Domain.Model
{
    public class TrnMapUnitChangeRequest:Common
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ChangeMapUnitId { get; set; }

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
        public int? ToUpdatedby { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        public DateTime? ToUpdatedOn { get; set; }
        
        [ForeignKey("ToMUserProfile"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? ToUserId { get; set; }
        public MUserProfile? ToMUserProfile { get; set; }
        public bool IsEditAction { get; set; }

    }
}
