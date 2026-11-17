using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Constants
{
    public enum MPostingReasonEnum:byte
    {
        PostingOut = 1,
        Death_ApplClose=2,
        Discharge_ApplClose=3,
        Dismissal_ApplClose=4,
        PermtClosedAppl=5,
        PermtPostingOutTfr=6,
        ToLongCourseTfr=7,
        LostCard=8,
    }
}
