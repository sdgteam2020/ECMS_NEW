using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOCardDistributionRequest
    {
        public int RequestId { get; set; }
        public string ArmyNo { get; set; }
        public string CardSerialNo { get; set; }
        public string ChipNo { get; set; }
    }
}
