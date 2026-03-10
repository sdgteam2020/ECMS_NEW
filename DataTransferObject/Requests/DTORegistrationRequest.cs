using DataTransferObject.Localize;
using System.ComponentModel.DataAnnotations;

namespace DataTransferObject.Requests
{
    public class DTORegistrationRequest : DTOBasicDetailTempRequest
    {
        [Display(Name = "ServiceNo", ResourceType = typeof(Resource))]
        //[Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string ServiceNumber { get; set; } = string.Empty;

        [MinLength(8, ErrorMessage = "Minimum length of Army No is eight character.")]
        [MaxLength(9, ErrorMessage = "Maximum length of Army No is nine character.")]
        public string? OldServiceNo { get; set; } = string.Empty;

        [Display(Name = "SubmitType", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [Range(1, 2, ErrorMessage = "SubmitType must be 1 or 2.")]
        public int SubmitType { get; set; }

        [RegularExpression(@"^[\w\, ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? RemarksIds { get; set; }
    }
}
