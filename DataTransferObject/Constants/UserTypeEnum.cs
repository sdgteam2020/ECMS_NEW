using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Constants
{
    public enum UserTypeEnum :byte
    {
        [Display(Name = "User")]
        User = 1,

        [Display(Name = "IO")]
        IO = 2,

        [Display(Name = "RO / ORO")]
        RO_ORO = 3,

        [Display(Name = "AFSC Cell")]
        AFSCCell = 4,

        [Display(Name = "Export / Print")]
        ExportPrint = 5
    }
}
