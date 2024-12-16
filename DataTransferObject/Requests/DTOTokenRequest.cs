using DataTransferObject.Localize;
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
        [MinLength(8, ErrorMessage = "Minimum length of Offr Army No is eight character.")]
        [MaxLength(9, ErrorMessage = "Maximum length of Offr Army No is nine character.")]
        [RegularExpression(@"^[\w]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string ICNo { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }


        // When the code is published on IAM, these lines are commented.

        [Required]
        [DataType(DataType.Password)]
        
        //------------------- End Instructions----------------------
        //[RegularExpression(@"^[\w \?\@\#\$\%\&\*\=\\\/]*$", ErrorMessage = "This < >^| special chars not allowed for security reasons.")]
        public string Password { get; set; } = string.Empty;

        // When the code is published on IAM, these lines are commented.
            
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        
        //------------------- End Instructions----------------------
    }
}
