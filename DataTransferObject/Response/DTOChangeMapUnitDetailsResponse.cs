using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOChangeMapUnitDetailsResponse
    {
        public int ExistingCh_UnitType { get; set; }
        public int RequestCh_UnitType { get; set; }
        public int UnitMapId { get; set; }
        public string RequestBy { get; set; } = string.Empty;
        public string Remark { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public byte BdeId { get; set; }
        public string BdeName { get; set; } = string.Empty;
        public byte ComdId { get; set; }
        public string ComdName { get; set; } = string.Empty;
        public byte CorpsId { get; set; }
        public string CorpsName { get; set; } = string.Empty;
        public byte DivId { get; set; }
        public string DivName { get; set; } = string.Empty;
        public string Sus_no { get; set; } = string.Empty;
        public byte PsoId { get; set; }
        public string PSOName { get; set; } = string.Empty;
        public byte FmnBranchID { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public byte SubDteId { get; set; }
        public string SubDteName { get; set; } = string.Empty;
    }
}
