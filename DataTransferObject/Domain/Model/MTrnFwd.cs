using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Master;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Model
{
    public class MTrnFwd : Common
    { 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int TrnFwdId { get; set; }
        [ForeignKey("MTrnICardRequest"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RequestId { get; set; }
        public MTrnICardRequest? MTrnICardRequest { get; set; }


        [ForeignKey("MUserProfile")]
        public int ToUserId { get; set; }
        public MUserProfile? MUserProfile { get; set; }

        [ForeignKey("MUserProfileFrom")]
        public int FromUserId { get; set; }
        public MUserProfile? MUserProfileFrom { get; set; }

        [ForeignKey("ApplicationUser")]
        public int? FromAspNetUsersId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

        [ForeignKey("ApplicationUser1")]
        public int? ToAspNetUsersId { get; set; }
        public ApplicationUser? ApplicationUser1 { get; set; }

        [ForeignKey("MapUnit"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UnitId { get; set; } 
        public MapUnit? MapUnit { get; set; }
        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string? Remark { get; set; } = string.Empty;

        [ForeignKey("MTrnFwdStatus"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte FwdStatusId { get; set; }
        public MTrnFwdStatus? MTrnFwdStatus { get; set; }
        
        [ForeignKey("MTrnFwdType"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte TypeId { get; set; }
       
        public MTrnFwdType? MTrnFwdType { get; set; }

        [ForeignKey("MStepCounterStep"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte StepId { get; set; }
        public MStepCounterStep? MStepCounterStep { get; set; }

        public bool IsComplete { get; set; } = false;

        [Column(TypeName = "varchar(100)")]
        public string? RemarksIds { get; set; }
        public int? PostingOutId { get; set; }
        //[ForeignKey("Fwd"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        //public int FwdId { get; set; }

        //public MTrnFwd? Fwd { get; set; }
    }
}
