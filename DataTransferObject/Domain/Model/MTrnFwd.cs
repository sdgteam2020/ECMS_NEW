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
        public int RequestId { get; set; }
        public MTrnICardRequest? MTrnICardRequest { get; set; }
        public int FromUserId { get; set; }
        [ForeignKey("FromUserId")]
        public MUserProfile? MUserProfile { get; set; }
        public int? ToUserId { get; set; }
        [ForeignKey("ToUserId")]
        public MUserProfile? MUserProfile1 { get; set; }
        public int SusNo { get; set; }
        public string? Remark { get; set; } = string.Empty;
        public bool Status { get; set; } = false;
        public short TypeId { get; set; }
        public MTrnFwdType? MTrnFwdType { get; set; }

    }
}
