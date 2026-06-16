using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Constants
{
    public enum ApplSubmittedStatusEnum :byte
    {
        [Display(Name = "Drafted")]
        DraftedSavedApplication = 1,

        [Display(Name = "Submitted")]
        Submitted = 2,

        [Display(Name = "Rejected")]
        Rejected = 3,

        [Display(Name = "Complete")]
        Complete = 4,
    }
}
