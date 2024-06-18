using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOProfileManageResponse
    {
        public int UserId { get; set; }
        public string ArmyNo { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public bool IsToken { get; set; }
        public short RankId { get; set; }
        public string RankName { get; set; } = string.Empty;
        public byte ArmedId { get; set; }
        public string ArmedName { get; set; } = string.Empty;
        public string RankAbbreviation { get; set; } = string.Empty;
        public int Id { get; set; }
        public string? DomainId { get; set; }
        public bool IsTokenWaiver { get; set; }
        public string? ReasonTokenWaiver { get; set; }
    }
}
