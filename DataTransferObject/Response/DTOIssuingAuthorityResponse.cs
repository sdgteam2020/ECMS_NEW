using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOIssuingAuthorityResponse
    {
        public byte IssuingAuthorityId { get; set; }
        public string IssuingAuthorityName { get; set; } = string.Empty;
    }
}
