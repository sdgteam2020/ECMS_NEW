using DataTransferObject.Validation;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DataTransferObject.Requests
{
    public class DTOCSVFileRequest
    {
        //[AllowedExtensions(new string[] { ".csv" })]
        //[AllowedContentType(new string[] { "text/csv" })]
        //[MaxFileSize(5120, "CSVFile")]
        [Required(ErrorMessage = "File is required!")]
        [SecureFile(allowedExtensions: new[] { ".csv" },
        allowedMimeTypes: new[] { "text/csv", "application/vnd.ms-excel" },
        expectedHeaders : new[] { "ApplId", "ServiceNo", "CardSerialNo", "ChipNo" },
        maxFileSize: 5 * 1024 * 1024)]
        public required IFormFile CSVFile { get; set; }
    }
}
