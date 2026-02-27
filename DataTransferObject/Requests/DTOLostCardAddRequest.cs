using DataTransferObject.Localize;
using DataTransferObject.Validation;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DataTransferObject.Requests
{
    public class DTOLostCardAddRequest
    {
        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int LostCardId { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int BasicDetailId { get; set; }

        [Required(ErrorMessage = "Application Id is required.")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int RequestId { get; set; }
        public List<int> RemarksIds { get; set; } = new List<int>();

        [Required(ErrorMessage = "Remark is required.")]
        [RegularExpression(@"^[\w \.]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [MaxLength(100, ErrorMessage = "Maximum length of Remark is hundred character.")]
        public string Remark { get; set; }=string.Empty;

        [Required]
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool IsFIRLogged { get; set; }
        
        [RegularExpression(@"^[A-Za-z0-9_ ]+$",ErrorMessage = "Only alphabets, numbers, space and underscore are allowed.")]
        public string? SupportDocPath { get; set; } = string.Empty;

        [RegularExpression(@"^[\w \.]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string SupportDocName { get; set; } = string.Empty;

        [RegularExpression(@"^[A-Za-z0-9+/=\s]+$", ErrorMessage = "Invalid string.")]
        public string? SignedXML { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lost date is required.")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        [LostDateValidation]
        public DateTime LostOn { get; set; }

        [SecureFile(allowedExtensions: new[] { ".pdf" },
        allowedMimeTypes: new[] { "application/pdf" },
        expectedHeaders: null,
        maxFileSize: 5 * 1024 * 1024)]
        public IFormFile? File { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int UpdatedbyUserId { get; set; }
        
        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public byte StatusId { get; set; }

        [RegularExpression(@"^[\w \.]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string AppointmentName { get; set; } = string.Empty;

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int? HotlistCardId { get; set; }

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool IsActive { get; set; } = true;

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int Updatedby { get; set; }

        public DateTime? UpdatedOn { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

    }
}
