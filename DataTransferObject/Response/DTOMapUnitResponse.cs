using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{ 
    public class DTOMapUnitResponse
    { 
        public int UnitMapId { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public string UnitAbbreviation { get; set; } = string.Empty;
        public bool IsVerify { get; set; }
        public int BdeId { get; set; }
        public string BdeName { get; set; } = string.Empty;
        public int ComdId { get; set; }
        public string ComdName { get; set; } = string.Empty;
        public int CorpsId { get; set; }
        public string CorpsName { get; set; } = string.Empty;
        public int DivId { get; set; }
        public string DivName { get; set; } = string.Empty;
        public string Sus_no { get; set; } = string.Empty;
        public int UnitType { get; set; }
        public string Suffix { get; set; } = string.Empty;
        public byte PsoId { get; set; }
        public string PSOName { get; set; } = string.Empty;
        public byte FmnBranchID { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public byte SubDteId { get; set; }
        public string SubDteName { get; set; } = string.Empty;
    }
}
