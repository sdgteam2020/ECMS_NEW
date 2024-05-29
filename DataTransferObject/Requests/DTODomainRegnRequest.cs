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
    public class DTODomainRegnRequest
    {
        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string DomainId { get; set; } = string.Empty;
        
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public required List<int> RoleIds { get; set; }

        [StringLength(6)]
        [Column(TypeName = "varchar(6)")]
        [Required(ErrorMessage = "ASCON Dialing  is required.")]
        [MinLength(6, ErrorMessage = "Minimum length of ASCON Dialing is six digit.")]
        [MaxLength(6, ErrorMessage = "Maximum length of ASCON Dialing is six digit.")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "Dialing code is invalid.")]
        public string DialingCode { get; set; } = string.Empty;

        [StringLength(5)]
        [Column(TypeName = "varchar(5)")]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [MinLength(4, ErrorMessage = "Minimum length of Extension is four digit.")]
        [MaxLength(5, ErrorMessage = "Maximum length of Extension is five digit.")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "Extension is invalid.")]
        public string Extension { get; set; } = string.Empty;

        [RegularExpression(@"^[a-zA-Z]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public bool AdminFlag { get; set; }

        [RegularExpression(@"^[a-zA-Z]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public bool Active { get; set; }

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool IsIO { get; set; }

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool IsCO { get; set; }

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool IsRO { get; set; }

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool IsORO { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        public int TDMId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        public int UnitMappId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        public short ApptId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        public int UserId { get; set; }
        
        [RegularExpression(@"^[\w\-]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? ArmyNo { get; set; }
        public int Updatedby { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
