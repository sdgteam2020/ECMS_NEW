using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOCardPriningRequest
    {
        public string ApplId { get; set; } = string.Empty;
        public string ServiceNo { get; set; } = string.Empty;
        public string CardSerialNo { get; set; } = string.Empty;
        public string ChipNo { get; set; } = string.Empty;
        public int CardPrintedByAspNetUserId { get; set; }
        public int CardPrintedByUserId { get; set; }
        public bool IsValid { get; set; } = true;
        public string Status { get; set; } = "Valid";
        public string Remarks { get; set; } = string.Empty;
    }
}
