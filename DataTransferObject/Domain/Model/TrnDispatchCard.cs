using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Identitytable;

namespace DataTransferObject.Domain.Model
{
    public class TrnDispatchCard : Common
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DispatchCardId { get; set; }

        public byte Step { get; set; } // 1 For AFSAC AND 2 FOR Regiment / Record

        [ForeignKey("MApplyFor"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte ApplyForId { get; set; }
        public MApplyFor? MApplyFor { get; set; }
        
        [ForeignKey("MRegimental"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte? RegId { get; set; }
        public MRegimental? MRegimental { get; set; }

        [ForeignKey("MRecordOffice"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte? RecordOfficeId { get; set; }
        public MRecordOffice? MRecordOffice { get; set; }
        public DateTime OutDate { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public DateTime DispatchDate { get; set; }
        
        [ForeignKey("MDispatchMode"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte DispatchModeId { get; set; }
        public MDispatchMode? MDispatchMode { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR(50)")]
        public string RefOfDispatch { get; set; } = string.Empty;
        
        [StringLength(50)]
        [Column(TypeName = "VARCHAR(50)")]
        public string LotNo { get; set; } = string.Empty;

        [StringLength(50)]
        [Column(TypeName = "VARCHAR(50)")]
        public string NameOfCourierIncharge { get; set; } = string.Empty;
        
        [StringLength(100)]
        [Column(TypeName = "VARCHAR(100)")]
        public string UploadFilePath { get; set; } = string.Empty;

        [StringLength(100)]
        [Column(TypeName = "VARCHAR(100)")]
        public string? FromRemark { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR(100)")]
        public string? ToRemark { get; set; }

        [ForeignKey("FromMapUnit")]
        public int FromUnitId { get; set; }
        public MapUnit? FromMapUnit { get; set; }

        [ForeignKey("ToMapUnit")]
        public int ToUnitId { get; set; }
        public MapUnit? ToMapUnit { get; set; }

        [ForeignKey("ToMUserProfile")]
        public int ToUserId { get; set; }
        public MUserProfile? ToMUserProfile { get; set; }

        [ForeignKey("FromMUserProfile")]
        public int FromUserId { get; set; }
        public MUserProfile? FromMUserProfile { get; set; }

        [ForeignKey("FromApplicationUser")]
        public int FromAspNetUsersId { get; set; }
        public ApplicationUser? FromApplicationUser { get; set; }

        [ForeignKey("ToApplicationUser")]
        public int ToAspNetUsersId { get; set; }
        public ApplicationUser? ToApplicationUser { get; set; }
        public bool IsComplete { get; set; } = false;
    }
}
