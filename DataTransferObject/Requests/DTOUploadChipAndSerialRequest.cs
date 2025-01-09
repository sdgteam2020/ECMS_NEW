using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOUploadChipAndSerialRequest
    {
        public required int RequestId { get; set; }
        public required string ChipNo { get; set; }
        public required string CardSerialNo { get; set; }
        public bool IsValid { get; set; }
    }
}
