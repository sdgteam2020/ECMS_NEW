using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOOROMappingResponse
    {
        public short OROMappingId { get; set; }
        public short? RankId { get; set; }
        public string? RankName { get; set; }
        public string? ArmedIdList { get; set; }
        public string? ArmNameList { get; set; }
        public byte RecordOfficeId { get; set; }
        public string RecordOfficeName { get; set; } = string.Empty;
        public int? TDMId { get; set; }
        public int? UnitId { get; set; }
        public string? DomainId { get; set; }
        public string? ArmyNo { get; set; }
        public string? RankAbbreviation { get; set; }
        public string? Name { get; set; }
        public string? Sus_no { get; set; }
        public string? Suffix { get; set; }
        public string? UnitName { get; set; }
    }
}
