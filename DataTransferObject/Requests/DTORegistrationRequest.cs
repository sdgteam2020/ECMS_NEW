using DataTransferObject.Localize;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTORegistrationRequest : DTOBasicDetailTempRequest
    {
        //public byte RegistrationId { get; set; }
        //public MRegistration? Registration { get; set; }

        //[RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        //public byte RegId { get; set; }

        [Display(Name = "ServiceNo", ResourceType = typeof(Resource))]
        //[Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string ServiceNumber { get; set; } = string.Empty;

        [Display(Name = "SubmitType", ResourceType = typeof(Resource))]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "SubmitType is number.")]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public int SubmitType { get; set; }

        [Required]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "ApplyForId is number.")]
        public byte ApplyForId { get; set; }

        [Required]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "RegistrationId is number.")]
        public byte RegistrationId { get; set; }

        [Required]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "TypeId is number.")]
        public byte TypeId { get; set; }

        [RegularExpression(@"^[\w\, ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? RemarksIds { get; set; }
    }
}
