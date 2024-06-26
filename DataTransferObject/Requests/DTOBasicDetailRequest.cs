﻿using DataTransferObject.Constants;
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
        [RegularExpression(@"^[\d]+$", ErrorMessage = "BasicDetailId is number.")]
        public int BasicDetailId { get; set; }
        public RegistrationType RegistrationType { get; set; }

        [Display(Name = "Name", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [MaxLength(36, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MaxLengthError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Rank", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "RankId is number.")]
        public int RankId { get; set; }
        public MRank? MRank { get; set; }

        [Display(Name = "ArmService", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "ArmedId is number.")]
        public int ArmedId { get; set; }
        public MArmedType? MArmedType { get; set; }

        [Remote(action: "IsServiceNoInUse", controller: "BasicDetail", AdditionalFields = "initialServiceNo")]
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
        [Range(typeof(int), "0", "120", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "HtError")]
        public int Height { get; set; } 

        [Display(Name = "AadhaarNo", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        //[RegularExpression(@"^[\d ]{14}$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "AadhaarNoDigit")]
        [RegularExpression(@"^[\d ]{4}$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "AadhaarNoDigit")]
        public string AadhaarNo { get; set; } = string.Empty;

        [Display(Name = "BloodGroup", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string BloodGroup { get; set; } = string.Empty;

        [Display(Name = "PlaceOfIssue", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? PlaceOfIssue { get; set; } = string.Empty;

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
        [Display(Name = "Signature_", ResourceType = typeof(Resource))]
        public IFormFile Signature_ { get; set; }

        [Display(Name = "SignatureImagePath", ResourceType = typeof(Resource))]
        public string? SignatureImagePath { get; set; } 

        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".gif" })]
        [AllowedContentType(new string[] { "image/jpeg", "image/jpg", "image/gif", "image/png" })]
        [MaxFileSize(200, "Photo")]
        [Display(Name = "Photo_", ResourceType = typeof(Resource))]
        public IFormFile Photo_ { get; set; }

        [Display(Name = "PhotoImagePath", ResourceType = typeof(Resource))]
        public string? PhotoImagePath { get; set; } 

        [Display(Name = "DateOfCommissioning", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public DateTime DateOfCommissioning { get; set; }

        [Display(Name = "PermanentAddress", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string PermanentAddress { get; set; } = string.Empty;
        public byte StatusLevel { get; set; }
        
        [Display(Name = "RegimentalId", ResourceType = typeof(Resource))]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "RegimentalId is number.")]
        public int? RegimentalId { get; set; }
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


        [NotMapped]
        public int StepCounter { get; set; } 
        [NotMapped]
        public int StepId { get; set; }       
        [NotMapped]
        [Display(Name = "Request For")]
        public string? ICardType { get; set; }
        [NotMapped]
        public int RequestId { get; set; }
    }
}
