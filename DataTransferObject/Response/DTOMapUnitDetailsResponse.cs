using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOMapUnitDetailsResponse
    {
        public int MapUnitChangeRequestId { get; set; }
        public int UnitMapId { get; set; }
        public string UnitAbbreviation { get; set; } = string.Empty;
        public string Sus_no { get; set; } = string.Empty;
        public string RankAbbreviation { get; set; } = string.Empty;
        public string RequestBy { get; set; } = string.Empty;
        public string ArmyNo { get; set; } = string.Empty;
        public string? AprovedBy { get; set; } 
        public string? AproverArmyNo { get; set; } 
        public string? AproverRankAbbreviation { get; set; } 
        public string Remark { get; set; } = string.Empty;
        public string? AdminRemark { get; set; }
        public bool IsComplete { get; set; } = false;
        public bool IsEditAction { get; set; }
        public bool RequestStatus { get; set; } = false;
        public DateTime? AdminUpdatedOn { get; set; }
        public int ExistingUnitType { get; set; }
        public string ExistingComdName { get; set; } = string.Empty;
        public string ExistingCorpsName { get; set; } = string.Empty;
        public string ExistingBdeName { get; set; } = string.Empty;
        public string ExistingDivName { get; set; } = string.Empty;
        public string ExistingBranchName { get; set; } = string.Empty;
        public string ExistingPSOName { get; set; } = string.Empty;
        public string ExistingSubDteName { get; set; } = string.Empty;
        public int RequestUnitType { get; set; }
        public byte ComdId { get; set; }
        public string RequestComdName { get; set; } = string.Empty;
        public byte CorpsId { get; set; }
        public string RequestCorpsName { get; set; } = string.Empty;
        public byte DivId { get; set; }
        public string RequestDivName { get; set; } = string.Empty;
        public byte BdeId { get; set; }
        public string RequestBdeName { get; set; } = string.Empty;
        public byte FmnBranchID { get; set; }
        public string RequestBranchName { get; set; } = string.Empty;
        public byte PsoId { get; set; }
        public string RequestPSOName { get; set; } = string.Empty;
        public byte SubDteId { get; set; }
        public string RequestSubDteName { get; set; } = string.Empty;
    }
}
