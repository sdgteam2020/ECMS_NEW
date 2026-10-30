using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Constants
{
    public enum MFwdTypeEnum:byte
    {
        User = 1,
        IO=2,
        RO_OR_ORO=3,
        AFSA_Cell=4,
        Export =5,
    }
}
