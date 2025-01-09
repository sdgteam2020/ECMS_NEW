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
        [AllowedExtensions(new string[] { ".csv" })]
        [AllowedContentType(new string[] { "text/csv" })]
        [MaxFileSize(5120, "CSVFile")]
        public required IFormFile CSVFile { get; set; }
    }
}
