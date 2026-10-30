using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Constants
{
    public enum MICardTypeEnum :byte
    {
        First_time = 1,
        Damaged = 2,
        Change_Rank = 3,
        Change_ArmyNo = 4,
        LostCard = 5,
    }
}
