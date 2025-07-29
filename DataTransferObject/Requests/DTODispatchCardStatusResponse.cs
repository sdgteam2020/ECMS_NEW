using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTODispatchCardStatusResponse
    {
        public int TotalFilteredRecords { get; set; }
        public int RequestId { get; set; }
        public byte StepId { get; set; }
        public byte ApplyForId { get; set; }
        public string ApplyFor { get; set; } = string.Empty;
        public string NameAsPerRecord { get; set; } = string.Empty;
        public string FName { get; set; } = string.Empty;
        public string? LName { get; set; }
        public string RankName { get; set; } = string.Empty; 
        public string Name { get; set; } = string.Empty;
        public string ServiceNo { get; set; } = string.Empty;
        public string ArmedAbbreviation { get; set; } = string.Empty;
        public byte? RegId { get; set; }
        public string? RegimentalName { get; set; }
        public byte? RecordOfficeId { get; set; }
        public string? RecordOfficeName { get; set; }
        public string ChipNo { get; set; } = string.Empty;
        public string CardSerialNo { get; set; } = string.Empty;
        public string SUSNo { get; set; } = string.Empty;
        public string UnitAbbreviation { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
