using DataTransferObject.Constants;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Master;
using DataTransferObject.Localize;
using DataTransferObject.Validation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Requests;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace DataTransferObject.ViewModels
{
    public class BasicDetailVM : Common
    {
        [Display(Name = "BasicDetailId", ResourceType = typeof(Resource))]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "BasicDetailId is number.")]
        public int BasicDetailId { get; set; }
        public int? PreviousBasicDetailId { get; set; }

        public bool IsLock { get; set; } = false;
        public int AspNetUsersId { get; set; }
        public byte StatusId { get; set; }

        [StringLength(12)]
        [MaxLength(12)]
        [Column(TypeName = "varchar(12)")]
        [RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string PaperIcardNo { get; set; } = string.Empty;
        
        [Display(Name = "Name", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [MaxLength(18, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MaxLengthError")]
        [RegularExpression(@"^[a-zA-Z ]*$", ErrorMessage = "Only Alphabets allowed.")]
        public string FName { get; set; } = string.Empty;

        [Display(Name = "Name", ResourceType = typeof(Resource))]
        [MaxLength(18, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MaxLengthError")]
        [RegularExpression(@"^[a-zA-Z ]*$", ErrorMessage = "Only Alphabets allowed.")]
        public string? LName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [MaxLength(36, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MaxLengthError")]
        [RegularExpression(@"^[a-zA-Z ]*$", ErrorMessage = "Only Alphabets allowed.")]
        public string NameAsPerRecord { get; set; } = string.Empty;

        [Display(Name = "Rank", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "RankId is number.")]
        public short RankId { get; set; }

        [RegularExpression(@"^[\w ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        public string? RankName { get; set; }

        [Display(Name = "ArmService", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "ArmedId is number.")]
        public byte ArmedId { get; set; }

        [RegularExpression(@"^[\w ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        public string? ArmedName { get; set; }

        [Display(Name = "ServiceNo", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [MinLength(8, ErrorMessage = "Minimum length of Army No is eight character.")]
        [MaxLength(9, ErrorMessage = "Maximum length of Army No is nine character.")]
        public string ServiceNo { get; set; } = string.Empty;

        public string? OldServiceNo { get; set; } = string.Empty;

        [Display(Name = "DOB", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }

        [Display(Name = "Height", ResourceType = typeof(Resource))]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "Height is number.")]
        [Range(typeof(int), "100", "250", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "HtError")]
        public int Height { get; set; }

        [Display(Name = "AadhaarNo", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[1-9][0-9]{11}$", ErrorMessage = "Aadhaar number must be exactly 12 digits and cannot start with 0.")]
        [MaxLength(12)]
        public string AadhaarNo { get; set; } = string.Empty;

        [Display(Name = "BloodGroup", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "BloodGroupId is number.")]
        public byte BloodGroupId { get; set; }
        public string? BloodGroup { get; set; }
        
        [Display(Name = "PlaceOfIssue", ResourceType = typeof(Resource))]
        [RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [MaxLength(50)]
        public string? PlaceOfIssue { get; set; }

        [Display(Name = "DateOfIssue", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [DataType(DataType.Date)]
        public DateTime DateOfIssue { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "UploadId is number.")]
        public int UploadId { get; set; }

        [Display(Name = "SignatureImagePath", ResourceType = typeof(Resource))]
        public string SignatureImagePath { get; set; } = string.Empty;

        [Display(Name = "PhotoImagePath", ResourceType = typeof(Resource))]
        public string PhotoImagePath { get; set; } = string.Empty;

        [Display(Name = "DateOfCommissioning", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [DataType(DataType.Date)]
        public DateTime DateOfCommissioning { get; set; }

        [Display(Name = "PermanentAddress", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w\-\.\,\&\/ ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string PermanentAddress { get; set; } = string.Empty;
        public byte StatusLevel { get; set; }

        [Display(Name = "RegimentalId", ResourceType = typeof(Resource))]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "RegimentalId is number.")]
        public byte? RegimentalId { get; set; }
        public MRegimental? Regimental { get; set; }

        [Required]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "ApplyForId is number.")]
        public byte ApplyForId { get; set; }
        
        [Required]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "RegistrationId is number.")]
        public byte RegistrationId { get; set; }
        
        [Required]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "TypeId is number.")]
        public byte TypeId { get; set; }

        [NotMapped]
        public string ApplyFor { get; set; } = string.Empty;
        
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "AddressId is number.")]
        public int AddressId { get; set; }

        //[RegularExpression(@"^[\w\& ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string State { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        //[RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string District { get; set; } = string.Empty;

        [Display(Name = "Police Station")]
        //[Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        //[RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? PS { get; set; }

        [DisplayName("Post Office")]
        //[RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [MaxLength(50, ErrorMessage = "Maximum length of Post Office is fifty character.")]
        public string? PO { get; set; }

        //[RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [MaxLength(50, ErrorMessage = "Maximum length of Tehsil is fifty character.")]
        public string? Tehsil { get; set; } 

        [RegularExpression(@"^[\w\-\.\/ ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [MaxLength(50, ErrorMessage = "Maximum length of Village is fifty character.")]
        public string? Village { get; set; }

        //[ValidInteger("Pin Code")]
        //[Required(ErrorMessage = "Pin Code is required")]
        //[Range(typeof(int), "100000", "999999", ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int? PinCode { get; set; }
        /// <summary>
        /// end address
        /// </summary>
        /// 
        public int InfoId { get; set; }
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [StringLength(200)]
        [MaxLength(200)]
        [Column(TypeName = "VARCHAR(200)")]
        public string IdenMark1 { get; set; } = string.Empty;
       
        public string? IdenMark2 { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public int UnitId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [Display(Name = "IssuingAuth", ResourceType = typeof(Resource))]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "IssuingAuthorityId is number.")]
        public byte IssuingAuthorityId { get; set; }

        [NotMapped]
        [Display(Name = "IssuingAuth", ResourceType = typeof(Resource))]
        public string IssuingAuthorityName { get; set; } = string.Empty;

        [NotMapped]
        public string UnitName { get; set; } = string.Empty;

        public bool IsSubmit { get; set; }
        
        [NotMapped]
        public int Type { get; set; }

        [NotMapped]
        public bool success { get; set; }

        [NotMapped]
        public string? message { get; set; }

        [NotMapped]
        public string? EncryptedId { get; set; }

        [NotMapped]
        public string? EncryptedRequestId { get; set; }

        [NotMapped]
        [Display(Name = "Sno", ResourceType = typeof(Resource))]
        public int Sno { get; set; }


        [NotMapped]
        public int StepCounter { get; set; }
        [NotMapped]
        public int StepId { get; set; }
        [NotMapped]
        [Display(Name = "Request For")]
        public string? ICardType { get; set; }
        [NotMapped]
        public int RequestId { get; set; }
        
        [NotMapped]
        public int IsTrnFwdId { get; set; }

        [NotMapped]
        public byte IsFwdStatusId { get; set; }
        public string? Remark { get; set; }
        [NotMapped]
        public string? TrackingId { get; set; }
        [NotMapped]
        public int IsPosting { get; set; }
        
        [NotMapped]
        public int RegistrationApplyFor { get; set; }
        
        [NotMapped]
        public string? ModifiedServiceNo { get; set; }

        public string? RegimentalName { get; set; }
    }
    public class BasicDetailCrtAndUpdVM : BasicDetailVM
    {
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".gif" })]
        [AllowedContentType(new string[] { "image/jpeg", "image/jpg", "image/gif", "image/png" })]
        [MaxFileSize(200, "Photo")]
        [Display(Name = "Photo_", ResourceType = typeof(Resource))]
        public IFormFile? Photo_ { get; set; }

        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".gif" })]
        [AllowedContentType(new string[] { "image/jpeg", "image/jpg", "image/gif", "image/png" })]
        [MaxFileSize(50, "Signature")]
        [Display(Name = "Signature_", ResourceType = typeof(Resource))]
        public IFormFile? Signature_ { get; set; }

        public string? ExistingSignatureImagePath { get; set; }
        public string? ExistingSignatureInBase64 { get; set; }
        public string? ExistingPhotoImagePath { get; set; }
        public string? ExistingPhotoInBase64 { get; set; }

        [Display(Name = "TermsConditions", ResourceType = typeof(Resource))]
        //[Range(typeof(bool), "true", "true", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "TermsConditionsError")]
        //[CheckBoxRequired(ErrorMessage = "Please accept the terms and condition.")]
        public bool TermsConditions { get; set; }
    }
}
