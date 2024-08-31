using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOFwdLastRecForDigitalSign
    {
        public string? FromDomain { get; set; }
        public string? FromProfile { get; set; }
        public string? FromRank { get; set; }
        public string FromArmyNo { get; set; } = string.Empty;  
        public DateTime? FromDate { get; set; }
        public int? StepId { get; set; }
    }
}
