using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOCardPriningRequest
    {
        public string ApplId { get; set; }
        public string ServiceNo { get; set; }
        public string CardSerialNo { get; set; }
        public string ChipNo { get; set; }
        public bool IsValid { get; set; } = true;
        public string Status { get; set; } = "Valid";
        public string Remarks { get; set; } = string.Empty;
    }
}
