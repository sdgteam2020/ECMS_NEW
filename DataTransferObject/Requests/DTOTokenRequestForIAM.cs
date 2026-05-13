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
        [Display(Name = "ArmyNumber", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        // Validation not implemented on ICNo because this field is encrypted and the format is not known. The validation will be done using ArmyNoHelper.ValidateArmyNo.
        //[MinLength(8, ErrorMessage = "Minimum length of Offr Army No is eight character.")]
        //[MaxLength(9, ErrorMessage = "Maximum length of Offr Army No is nine character.")]
        //[RegularExpression(@"^[\w]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string ICNo { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }

        //[RegularExpression(@"^[\w \?\@\#\$\%\&\*\=\\\/]*$", ErrorMessage = "This < >^| special chars not allowed for security reasons.")]
        public string Password { get; set; } = string.Empty;
    }
}
