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
     
        public int FromUserId { get; set; }
        public int? ToUserId { get; set; }
        public int SusNo { get; set; }
        public string? Remark { get; set; } = string.Empty;
        public bool Status { get; set; } = false;
        public int HType { get; set; }

    }
}
