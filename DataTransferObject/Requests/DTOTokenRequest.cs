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
        [MaxLength(10, ErrorMessage = "Maximum length of Offr Army No is ten character.")]
        [RegularExpression(@"^[\w]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string ICNo { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }
    }
}
