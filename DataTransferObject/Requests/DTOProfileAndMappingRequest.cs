using DataTransferObject.Domain.Master;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Localize;

namespace DataTransferObject.Requests
{
    public class DTOProfileAndMappingRequest
    {
        [RegularExpression(@"^[\d]+$", ErrorMessage = "UserId is number.")]
        public int UserId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [MinLength(8, ErrorMessage = "Minimum length of Offr Army No is eight character.")]
        [MaxLength(10, ErrorMessage = "Maximum length of Offr Army No is ten character.")]
        [RegularExpression(@"^[\w]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string ArmyNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Rank is required.")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "RankId is number.")]
        public short RankId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [MaxLength(50, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MaxLengthError")]
        [MinLength(1, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MinLengthError")]
        [RegularExpression(@"^[\w ]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile No  is required.")]
        [MinLength(10, ErrorMessage = "Minimum length of Mobile No is ten digit.")]
        [MaxLength(10, ErrorMessage = "Maximum length of Mobile No is ten digit.")]
        [RegularExpression(@"^[\d]+$", ErrorMessage ="Mobile No. is invalid.")]
        public string MobileNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "ASCON Dialing  is required.")]
        [MinLength(6, ErrorMessage = "Minimum length of ASCON Dialing is six digit.")]
        [MaxLength(6, ErrorMessage = "Maximum length of ASCON Dialing is six digit.")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "Dialing code is invalid.")]
        public string DialingCode { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [MinLength(4, ErrorMessage = "Minimum length of Extension is four digit.")]
        [MaxLength(5, ErrorMessage = "Maximum length of Extension is five digit.")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "Extension is invalid.")]
        public string Extension { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "ApptId is number.")]
        public short ApptId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "UnitMapId is number.")]
        public int UnitMapId { get; set; }

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public Boolean IntOffr { get; set; }

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public Boolean IsIO { get; set; }

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public Boolean IsCO { get; set; }

    }
}
