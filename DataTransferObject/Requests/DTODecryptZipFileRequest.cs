using DataTransferObject.Localize;
using DataTransferObject.Validation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTODecryptZipFileRequest
    {
        [AllowedExtensions(new string[] { ".zip" })]
        [AllowedContentType(new string[] { "application/x-zip-compressed" })]
        [MaxFileSize(5120, "ZipFile")]
        public required IFormFile ZipFile { get; set; }
        public string PrivateKey { get; set; } = string.Empty;
    }
}
