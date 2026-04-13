using DataTransferObject.Localize;
using DataTransferObject.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOTokenRequest
    {
        [Display(Name = "ArmyNumber", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        // Validation not implemented on ICNo because this field is encrypted and the format is not known. The validation will be done using ArmyNoHelper.ValidateArmyNo.
        public string ICNo { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }

        // Validation not implemented on Password because this field is encrypted and the format is not known. The validation will be done using ArmyNoHelper.ValidateArmyNo.
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;


        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
