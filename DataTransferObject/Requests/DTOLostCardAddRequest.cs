using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Domain.Model;
using Microsoft.AspNetCore.Http;
using DataTransferObject.Validation;

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
