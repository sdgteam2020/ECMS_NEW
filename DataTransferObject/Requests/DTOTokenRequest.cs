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
        [MinLength(8, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MinLengthError")]
        [MaxLength(8, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MaxLengthError")]
        [RegularExpression(@"^[\w]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string ICNo { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }
    }
}
