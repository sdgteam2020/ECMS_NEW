using DataTransferObject.Validation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOCSVFileRequest
    {
        //[AllowedExtensions(new string[] { ".csv" })]
        //[AllowedContentType(new string[] { "text/csv" })]
        //[MaxFileSize(5120, "CSVFile")]
        [SecureFile(allowedExtensions: new[] { ".csv" },
        allowedMimeTypes: new[] { "text/csv", "application/vnd.ms-excel" },
        expectedHeaders : new[] { "RequestId", "ServiceNo", "CardSerialNo", "ChipNo" },
        maxFileSize: 5 * 1024 * 1024)]
        public required IFormFile CSVFile { get; set; }
    }
}
