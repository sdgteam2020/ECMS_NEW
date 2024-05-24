using DataTransferObject.Localize;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOUpdateProfileWithMappingRequest
    {
        [RegularExpression(@"^[\d]+$", ErrorMessage = "UserId is number.")]
        public int UserId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "TDMId is number.")]
        public int TDMId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [MaxLength(50, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MaxLengthError")]
        public string Name { get; set; } = string.Empty;

        [RegularExpression(@"^[\d]+$", ErrorMessage = "RankId is number.")]
        public short RankId { get; set; }

        [Required(ErrorMessage = "Mobile No  is required.")]
        [MinLength(10, ErrorMessage = "Minimum length of Mobile No is ten digit.")]
        [MaxLength(10, ErrorMessage = "Maximum length of Mobile No is ten digit.")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "Mobile No. is invalid.")]
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

        [RegularExpression(@"^[\w]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? Thumbprint { get; set; }

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool IsToken { get; set; }

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool IsRO { get; set; }

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool IsIO { get; set; }

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool IsCO { get; set; }

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool IsORO { get; set; }

        public int Updatedby { get; set; }

        [DataType(DataType.Date)]
        public DateTime? UpdatedOn { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
    }
}
