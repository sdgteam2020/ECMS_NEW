using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOAllRelatedDataByArmyNoResponse
    {
        public int UserId { get; set; }
        public string ArmyNo { get; set; } = string.Empty;
        public int RankId { get; set; }
        public string RankName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public short ApptId { get; set; }
        public string AppointmentName { get; set; } = string.Empty;
        public int UnitId { get; set; }
        public string? UnitName { get; set; }
        public int TrnDomainMappingId { get; set; }
        public string? DomainId { get; set; }
        public string? AdminMsg { get; set; }
        public bool IsRO { get; set; }
        public bool IsIO { get; set; }
        public bool IsCO { get; set; }
        public bool IsORO { get; set; }
        public string DialingCode { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
    }
}
