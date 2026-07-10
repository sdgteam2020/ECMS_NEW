using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Constants
{
    public enum ApplyForEnum : byte
    {
        [Display(Name = "Offrs")]
        Officers = 1,

        [Display(Name = "JCO/ORs")]
        JCO_ORs = 2,

        [Display(Name = "Other")]
        Other = 3
    }
}
