using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOICardTypeRequest
    {
        public byte TypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string EncryptedId { get; set; } = string.Empty;
    }
}
