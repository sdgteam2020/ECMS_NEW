using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Constants
{
    public enum NotificationDisplayEnum:byte
    {
        IOReject = 7,
        ORORReject = 8,
        AFSACRejectOffr = 9,
        PrintRejectOffr = 10,
        COReject = 17,
        ROReject = 18,
        AFSACRejectJCOOR = 19,
        PrintRejectJCOOR = 20,
    }
}

