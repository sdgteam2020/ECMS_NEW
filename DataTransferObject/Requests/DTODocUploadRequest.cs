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
using DataTransferObject.Domain.Model;
using DataTransferObject.Domain;

namespace DataTransferObject.Requests
{
    public class DTODocUploadRequest:Common
    {
        [Display(Name = "DocUploadId", ResourceType = typeof(Resource))]
        public int DocUploadId { get; set; }

        [Display(Name = "DocName", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string DocName { get; set; } = string.Empty;

        [AllowedExtensions(new string[] { ".docx" })]
        [AllowedContentType(new string[] { "application/vnd.openxmlformats-officedocument.wordprocessingml.document" })]
        [MaxFileSize(512, "Document")]
        [Display(Name = "DocPath", ResourceType = typeof(Resource))]
        public IFormFile Doc_ { get; set; }

        [Display(Name = "DocPath", ResourceType = typeof(Resource))]
        public string DocPath { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }

        [NotMapped]
        public string? EncryptedId { get; set; }
    }
    public class DTODocUploadCrtRequest : DTODocUploadRequest
    {

    }
    public class DTODocUploadUpdRequest
    {
        [Display(Name = "DocUploadId", ResourceType = typeof(Resource))]
        public int DocUploadId { get; set; }

        [Display(Name = "DocName", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string DocName { get; set; } = string.Empty;

        [AllowedExtensions(new string[] { ".doc", ".docx" })]
        [AllowedContentType(new string[] { "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" })]
        [MaxFileSize(512, "Document")]
        [Display(Name = "DocPath", ResourceType = typeof(Resource))]
        public IFormFile? Doc_ { get; set; }

        [Display(Name = "DocPath", ResourceType = typeof(Resource))]
        public string DocPath { get; set; } = string.Empty;

        public string ExistingDocPath { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }

        [NotMapped]
        public string? EncryptedId { get; set; }
    }
}
