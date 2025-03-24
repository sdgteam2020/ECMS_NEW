using AutoMapper.Configuration.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOCardDistributionRequest
    {
        public string RequestId { get; set; }
        public string ArmyNo { get; set; }
        public string CardSerialNo { get; set; }
        public string ChipNo { get; set; }
        [Ignore]
        public bool IsValid { get; set; } = true;
        public string Remarks { get; set; } = string.Empty;
    }
}
