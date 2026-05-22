using DataTransferObject.Localize;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOTokenRequestForIAM
    {
        // Validation not implemented on ICNo because this field is encrypted and the format is not known. The validation will be done using ArmyNoHelper.ValidateArmyNo.
        [Display(Name = "ArmyNumber", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public string ICNo { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }

        public string Password { get; set; } = string.Empty;
    }
}
