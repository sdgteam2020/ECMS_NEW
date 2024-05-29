using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Domain.Identitytable;

namespace DataTransferObject.Requests
{
    public class DTOSaveInternalFwdRequest
    {
        public required int[] TrnFwdIds { get; set; }
        public int ToUserId { get; set; }
        public int FromUserId { get; set; }
        public int? FromAspNetUsersId { get; set; }
        public int? ToAspNetUsersId { get; set; }
        public int UnitId { get; set; }

        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string? Remark { get; set; } = string.Empty;
        public byte FwdStatusId { get; set; }
        public byte TypeId { get; set; }
        public bool IsComplete { get; set; } = false;

        [Column(TypeName = "varchar(100)")]
        public string? RemarksIds { get; set; }
        public bool IsActive { get; set; } = true;
        public int? Updatedby { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
