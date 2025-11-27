using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOGetTokenArmyNoResponse
    {
        public string ICNO { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string RankName { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
    }
}
