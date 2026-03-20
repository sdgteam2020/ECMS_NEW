using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTORsaKeyResponse
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
    }
}
