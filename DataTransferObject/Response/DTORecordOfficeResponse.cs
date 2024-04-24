using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTORecordOfficeResponse
    {
        public byte RecordOfficeId { get; set; }
        public string RecordOfficeName { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;
        public byte ArmedId { get; set; }
        public string ArmedName { get; set; } = string.Empty;
        public int? TDMId { get; set; }
        public int? UnitId { get; set; }
        public string? Message { get; set; }
        public string? DomainId { get; set; } 
        public string? ArmyNo { get; set; } 
        public string? RankAbbreviation { get; set; } 
        public string? Name { get; set; }
        public string? Sus_no { get; set; }
        public string? Suffix { get; set; }
        public string? UnitName { get; set; } 
    }
}
