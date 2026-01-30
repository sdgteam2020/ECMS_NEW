using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOMapUnitChangeResponse
    {
        public int MapUnitChangeRequestId { get; set; }
        public string? EncryptedId { get; set; }
        public int UnitMapId { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public string Sus_no { get; set; } = string.Empty;
        public string Suffix { get; set; } = string.Empty;
        public string FromArmyNo { get; set; } = string.Empty;
        public string FromRankAbbreviation { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
        public string FromDID { get; set; } = string.Empty;
        public DateTime RequestTime { get; set; }
        public string Remark { get; set; } = string.Empty;
        public string? AdminRemark { get; set; }
        public string ExistingCh { get; set; } = string.Empty;
        public string RequestCh { get; set; } = string.Empty;
        public int FromUpdatedby { get; set; }
        public DateTime FromUpdatedOn { get; set; }
        public int FromUserId { get; set; }
        public int? AdminUpdatedby { get; set; }
        public DateTime? AdminUpdatedOn { get; set; }
        public int? AdminUserId { get; set; }
        public bool IsComplete { get; set; } = false;
        public bool IsEditAction { get; set; }
        public bool IsActive { get; set; } = true;
        public bool RequestStatus { get; set; } = false;
    }
}
