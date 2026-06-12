using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Constants
{
    public enum RequestStatusEnum : byte
    {
        [Display(Name = "Running")]
        Running = 1,

        [Display(Name = "Complete")]
        Complete = 2,

        [Display(Name = "Closed")]
        Closed = 3
    }
}
