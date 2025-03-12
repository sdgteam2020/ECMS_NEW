using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOClaimsStoreResponse
    {
        public int ClaimStoreId { get; set; }
        public string ClaimType { get; set; } = string.Empty;
        public int TotalUsers { get; set; }
    }
}
