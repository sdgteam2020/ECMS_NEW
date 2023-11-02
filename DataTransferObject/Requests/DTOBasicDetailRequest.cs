using DataTransferObject.Domain;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Localize;
using DataTransferObject.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOBasicDetailRequest: Common
    {
        [Display(Name = "BasicDetailId", ResourceType = typeof(Resource))]
        public int BasicDetailId { get; set; }

        [Display(Name = "Name", ResourceType = typeof(Resource))]
        //[Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [MaxLength(36, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MaxLengthError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? Name { get; set; } 

        [Display(Name = "Rank", ResourceType = typeof(Resource))]
        //[Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? Rank { get; set; } 

        [Display(Name = "ArmService", ResourceType = typeof(Resource))]
        //[Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? ArmService { get; set; } 

        [Remote(action: "IsServiceNoInUse", controller: "BasicDetail", AdditionalFields = "initialServiceNo")]
        [Display(Name = "ServiceNo", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string ServiceNo { get; set; } = string.Empty;

        [Display(Name = "IdentityMark", ResourceType = typeof(Resource))]
        //[Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [MaxLength(20, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MaxLengthError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? IdentityMark { get; set; } 

        [Display(Name = "DOB", ResourceType = typeof(Resource))]
        //[Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public DateTime? DOB { get; set; }

        [Display(Name = "Height", ResourceType = typeof(Resource))]
        //[Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [Range(typeof(decimal), "0", "120", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "HtError")]
        public decimal? Height { get; set; } 

        [Display(Name = "AadhaarNo", ResourceType = typeof(Resource))]
        //[Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\d ]{14}$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "AadhaarNoDigit")]
        public string? AadhaarNo { get; set; } 

        [Display(Name = "BloodGroup", ResourceType = typeof(Resource))]
        //[Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? BloodGroup { get; set; } 

        [Display(Name = "PlaceOfIssue", ResourceType = typeof(Resource))]
        //[Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? PlaceOfIssue { get; set; } 

        [Display(Name = "DateOfIssue", ResourceType = typeof(Resource))]
        //[Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public DateTime? DateOfIssue { get; set; }

        [Display(Name = "IssuingAuth", ResourceType = typeof(Resource))]
        //[Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? IssuingAuth { get; set; } 

        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".gif" })]
        [AllowedContentType(new string[] { "image/jpeg", "image/jpg", "image/gif", "image/png" })]
        [MaxFileSize(50, "Signature")]
        [Display(Name = "SignatureImagePath", ResourceType = typeof(Resource))]
        public IFormFile? Signature_ { get; set; }

        [Display(Name = "SignatureImagePath", ResourceType = typeof(Resource))]
        public string? SignatureImagePath { get; set; } 

        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".gif" })]
        [AllowedContentType(new string[] { "image/jpeg", "image/jpg", "image/gif", "image/png" })]
        [MaxFileSize(200, "Photo")]
        [Display(Name = "PhotoImagePath", ResourceType = typeof(Resource))]
        public IFormFile? Photo_ { get; set; }

        [Display(Name = "PhotoImagePath", ResourceType = typeof(Resource))]
        public string? PhotoImagePath { get; set; } 

        [Display(Name = "DateOfCommissioning", ResourceType = typeof(Resource))]
        //[Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public DateTime? DateOfCommissioning { get; set; }

        [Display(Name = "PermanentAddress", ResourceType = typeof(Resource))]
        //[Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? PermanentAddress { get; set; } 

        [Display(Name = "StateId", ResourceType = typeof(Resource))]
        //[Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public int? StateId { get; set; }
        public State? State { get; set; }

        [Display(Name = "DistrictId", ResourceType = typeof(Resource))]
        //[Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public int? DistrictId { get; set; } 
        public District? District { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsSubmit { get; set; }

        [NotMapped]
        public bool success { get; set; }

        [NotMapped]
        public string? message { get; set; }

        [NotMapped]
        public string? EncryptedId { get; set; }
        
        [NotMapped]
        [Display(Name = "Sno", ResourceType = typeof(Resource))]
        public int Sno { get; set; }
    }
    public class DTOBasicDetailCrtRequest : DTOBasicDetailRequest
    {

        [Display(Name = "TermsConditions", ResourceType = typeof(Resource))]
        //[Range(typeof(bool), "true", "true", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "TermsConditionsError")]
        //[CheckBoxRequired(ErrorMessage = "Please accept the terms and condition.")]
        public bool TermsConditions { get; set; }
    }
    public class DTOBasicDetailUpdRequest
    {
        [Display(Name = "BasicDetailId", ResourceType = typeof(Resource))]
        public int BasicDetailId { get; set; }

        [Display(Name = "Name", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [MaxLength(36, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MaxLengthError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Rank", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string Rank { get; set; } = string.Empty;

        [Display(Name = "ArmService", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string ArmService { get; set; } = string.Empty;

        [Display(Name = "ServiceNo", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string ServiceNo { get; set; } = string.Empty;

        [Display(Name = "IdentityMark", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [MaxLength(20, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MaxLengthError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string IdentityMark { get; set; } = string.Empty;

        [Display(Name = "DOB", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public DateTime DOB { get; set; }

        [Display(Name = "Height", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [Range(typeof(decimal), "0", "120", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "HtError")]
        public decimal Height { get; set; }

        [Display(Name = "AadhaarNo", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\d ]{14}$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "AadhaarNoDigit")]
        //[RegularExpression(@"^\d{12}$", ErrorMessage = "AADHAAR Number is twelve digits.")]
        public string AadhaarNo { get; set; } = string.Empty;

        [Display(Name = "BloodGroup", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string BloodGroup { get; set; } = string.Empty;

        [Display(Name = "PlaceOfIssue", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string PlaceOfIssue { get; set; } = string.Empty;

        [Display(Name = "DateOfIssue", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public DateTime DateOfIssue { get; set; }

        [Display(Name = "IssuingAuth", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string IssuingAuth { get; set; } = string.Empty;

        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".gif" })]
        [AllowedContentType(new string[] { "image/jpeg", "image/jpg", "image/gif", "image/png" })]
        [MaxFileSize(50, "Signature")]
        public IFormFile? Signature_ { get; set; }

        public string SignatureImagePath { get; set; } = string.Empty;

        public string ExistingSignatureImagePath { get; set; } = string.Empty;

        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".gif" })]
        [AllowedContentType(new string[] { "image/jpeg", "image/jpg", "image/gif", "image/png" })]
        [MaxFileSize(200, "Photo")]
        public IFormFile? Photo_ { get; set; }

        public string PhotoImagePath { get; set; } = string.Empty;

        public string ExistingPhotoImagePath { get; set; } = string.Empty;

        [Display(Name = "DateOfCommissioning", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public DateTime DateOfCommissioning { get; set; }

        [Display(Name = "PermanentAddress", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string PermanentAddress { get; set; } = string.Empty;

        [Display(Name = "StateId", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public int StateId { get; set; }
        public State? State { get; set; }

        [Display(Name = "DistrictId", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public int DistrictId { get; set; }
        public District? District { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsSubmit { get; set; }

        [NotMapped]
        public string? EncryptedId { get; set; }
    }
    public class DTOBasicDetailTempRequest : Common
    {
        [Display(Name = "Name", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [MaxLength(36, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MaxLengthError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "ServiceNo", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string ServiceNo { get; set; } = string.Empty;

        [Display(Name = "DOB", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public DateTime DOB { get; set; }

        [Display(Name = "DateOfCommissioning", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public DateTime DateOfCommissioning { get; set; }

        [Display(Name = "PermanentAddress", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string PermanentAddress { get; set; } = string.Empty;

        [NotMapped]
        public string? EncryptedId { get; set; }

        [NotMapped]
        [Display(Name = "Sno", ResourceType = typeof(Resource))]
        public int Sno { get; set; }
    }
    public class DTORegistrationRequest : DTOBasicDetailTempRequest
    {
        public string RegistrationType { get; set; } = string.Empty;
        public string ServiceNumber { get; set; } = string.Empty;

        [Display(Name = "SubmitType", ResourceType = typeof(Resource))]
        [RegularExpression(@"^[\d ]{1}$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public int SubmitType { get; set; }

    }
    public class BasicDetailUpdVMPart1
    {
        public int BasicDetailId { get; set; }

        [Display(Name = "Name", ResourceType = typeof(Resource))]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "ServiceNo", ResourceType = typeof(Resource))]
        public string ServiceNo { get; set; } = string.Empty;

        [Display(Name = "DOB", ResourceType = typeof(Resource))]
        public DateTime DOB { get; set; }

        [Display(Name = "DateOfCommissioning", ResourceType = typeof(Resource))]
        public DateTime DateOfCommissioning { get; set; }

        [Display(Name = "PermanentAddress", ResourceType = typeof(Resource))]
        public string PermanentAddress { get; set; } = string.Empty;

        public int Step { get; set; }

        [NotMapped]
        public string EncryptedId { get; set; }

    }
    public class BasicDetailUpdVMPart2
    {
        public int BasicDetailId { get; set; }

        [Display(Name = "Rank", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string Rank { get; set; } = string.Empty;

        [Display(Name = "ArmService", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string ArmService { get; set; } = string.Empty;

        [Display(Name = "IdentityMark", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [MaxLength(20, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MaxLengthError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string IdentityMark { get; set; } = string.Empty;

        [Display(Name = "Height", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [Range(typeof(decimal), "0", "120", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "HtError")]
        public decimal Height { get; set; }

        [Display(Name = "AadhaarNo", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\d ]{14}$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "AadhaarNoDigit")]
        //[RegularExpression(@"^\d{12}$", ErrorMessage = "AADHAAR Number is twelve digits.")]
        public string AadhaarNo { get; set; } = string.Empty;

        [Display(Name = "BloodGroup", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string BloodGroup { get; set; } = string.Empty;

        [Display(Name = "PlaceOfIssue", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string PlaceOfIssue { get; set; } = string.Empty;

        [Display(Name = "DateOfIssue", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public DateTime DateOfIssue { get; set; }

        [Display(Name = "IssuingAuth", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string IssuingAuth { get; set; } = string.Empty;

        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".gif" })]
        [AllowedContentType(new string[] { "image/jpeg", "image/jpg", "image/gif", "image/png" })]
        [MaxFileSize(50, "Signature")]
        public IFormFile? Signature_ { get; set; }

        public string SignatureImagePath { get; set; } = string.Empty;

        public string ExistingSignatureImagePath { get; set; } = string.Empty;

        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".gif" })]
        [AllowedContentType(new string[] { "image/jpeg", "image/jpg", "image/gif", "image/png" })]
        [MaxFileSize(200, "Photo")]
        public IFormFile? Photo_ { get; set; }

        public string PhotoImagePath { get; set; } = string.Empty;

        public string ExistingPhotoImagePath { get; set; } = string.Empty;

        [Display(Name = "StateId", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public int StateId { get; set; }
        public State? State { get; set; }

        [Display(Name = "DistrictId", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public int DistrictId { get; set; }
        public District? District { get; set; }
        public int Step { get; set; }
        public DateTime UpdatedOn { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

        [ScaffoldColumn(false)]
        public int Updatedby { get; set; }

        [NotMapped]
        public string EncryptedId { get; set; }
    }

   

}
