using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Constants
{
    public enum DIDStatusEnum : byte
    {
        AdminFlagFalse = 1,
        NewDID = 2,
        MappingIdEmpty = 3,
        ProfileNotMapped = 4,
        ValidUser = 5,
        RoleNotAllowed = 6
    }
}
