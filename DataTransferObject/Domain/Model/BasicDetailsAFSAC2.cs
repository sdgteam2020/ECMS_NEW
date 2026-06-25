using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Model
{
    public class BasicDetailsAFSAC2
    {
        public int BasicDetailId { get; set; }

        public string FName { get; set; }=string.Empty;
        public string? LName { get; set; }
        public string ServiceNo { get; set; } = string.Empty;
        public int RankId { get; set; }
        public int UnitId { get; set; }
        public int ApplyForId { get; set; }
        public int ArmedId { get; set; }
        public int RegimentalId { get; set; }
    }
}
