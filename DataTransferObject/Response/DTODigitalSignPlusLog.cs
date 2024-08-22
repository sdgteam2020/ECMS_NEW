using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTODigitalSignPlusLog
    {
        public int Sno { get; set; }
        public int TypeLog { get; set; }
        public string? FromDomain { get; set; }
        public string? FromProfile { get; set; }
        public string? FromRank { get; set; }
        public string? FromArmyNo { get; set; }
        public DateTime? FromDate { get; set; }
        public string? LevelMessage { get; set; }
    }
}
