using AutoMapper.Configuration.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOCardPriningRequest
    {
        public string RequestId { get; set; }
        public string ServiceNo { get; set; }
        public string CardSerialNo { get; set; }
        public string ChipNo { get; set; }
        [Ignore]
        public bool IsValid { get; set; } = true;
        public string Status { get; set; } = "Valid";
        public string Remarks { get; set; } = string.Empty;
    }
}
