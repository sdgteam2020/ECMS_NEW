using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOApplFwdConditionRequest
    {
        public string MP6F { get; set; } = string.Empty;
        public string MPRSO { get; set; } = string.Empty;
        public List<string> ArmedAbbreviation { get; set; }= new();
        public string ArmyNoPrefix { get; set; } = string.Empty;
        public short RankOrderby { get; set; }
    }
}
