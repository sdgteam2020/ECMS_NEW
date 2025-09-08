using DataTransferObject.Validation;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DataTransferObject.Requests
{
    public class DTOLostCardAddRequest
    {
        [Required]
        public int RequestId { get; set; }
        [Required]
        public string? Remark { get; set; }
        [Required]
        public bool IsFIRLogged { get; set; }
        public string? SupportDocPath { get; set; } = string.Empty;
        public string? SignedXML { get; set; } = string.Empty;
        [Required]
        public DateTime? LostOn { get; set; }

        [SecureFile(allowedExtensions: new[] { ".pdf" },
        allowedMimeTypes: new[] { "application/pdf" },
        expectedHeaders: null,
        maxFileSize: 5 * 1024 * 1024)]
        public IFormFile? File { get; set; }
    }
}
