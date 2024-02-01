using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Master;
using DataTransferObject.Localize;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOSaveUnitWithMappingRequest
    {
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [MinLength(8, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MinLengthError")]
        [MaxLength(8, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MaxLengthError")]
        public string ServiceNo { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [MaxLength(50, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MaxLengthError")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [MaxLength(50, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MaxLengthError")]
        public string Rank { get; set; } = string.Empty;

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

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w\-]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [MaxLength(20, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MaxLengthError")]
        public string DomainId { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        //[RegularExpression("^[0-9]{7}[a-zA-Z]{1}$", ErrorMessage = "Use Seven digit numbers and one alphabet in capital letter without space")]
        [RegularExpression("^[0-9]{7}$", ErrorMessage = "Use Seven digit numbers")]
        [MaxLength(7, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MaxLengthError")]
        public string Sus_no { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression("^[A-Z]{1}$", ErrorMessage = "Alphabet in Capital letter.")]
        [MaxLength(1, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MaxLengthError")]
        public string Suffix { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        [MaxLength(100, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MaxLengthError")]
        public string UnitName { get; set; } = string.Empty;

        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        [Range(typeof(int), "1", "3", ErrorMessage = "Invalid Unit Type.")]
        public int UnitType { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public byte ComdId { get; set; }
        
        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public byte CorpsId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public byte DivId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public byte BdeId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public byte PsoId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public byte FmnBranchID { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public byte SubDteId { get; set; }

        public int Updatedby { get; set; }

        [DataType(DataType.Date)]
        public DateTime? UpdatedOn { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
    }
}
