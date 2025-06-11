using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using static System.Net.Mime.MediaTypeNames;

namespace DataTransferObject.Validation
{
    public class SecureFileAttribute : ValidationAttribute
    {
        private readonly string[] _allowedExtensions;
        private readonly string[] _allowedMimeTypes;
        private readonly long _maxFileSize;
        private readonly string[] _expectedHeaders;

        private static readonly string[] DangerousPatterns = new[]
        {
        "<script", "javascript:", "onerror=", "onload=", "eval(", "alert(", "<?php", "<iframe"
        };

        public SecureFileAttribute(string[] allowedExtensions, string[] allowedMimeTypes, string[] expectedHeaders, long maxFileSize)
        {
            _allowedExtensions = allowedExtensions ?? Array.Empty<string>();
            _allowedMimeTypes = allowedMimeTypes ?? Array.Empty<string>();
            _maxFileSize = maxFileSize;
            _expectedHeaders = expectedHeaders;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            //if (file == null)
            //    return new ValidationResult("File is required.");

            if (file != null)
            {
                if (file.Length == 0)
                    return new ValidationResult("File is empty.");

                if (file.Length > _maxFileSize)
                    return new ValidationResult("File exceeds the maximum allowed size.");

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_allowedExtensions.Contains(extension))
                    return new ValidationResult("File extension is not allowed.");

                if (!_allowedMimeTypes.Contains(file.ContentType))
                    return new ValidationResult($"Invalid content type: {file.ContentType}");

                if (_allowedExtensions.Contains(".csv"))
                {
                    using var headerReader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
                    var headerLine = headerReader.ReadLine();
                    if (string.IsNullOrWhiteSpace(headerLine))
                        return new ValidationResult("CSV file has no header.");


                    var actualHeaders = headerLine.Split(',').Select(h => h.Trim().ToLowerInvariant()).ToArray();
                    var requiredLower = _expectedHeaders.Select(h => h.Trim().ToLowerInvariant());

                    var missingHeaders = requiredLower.Where(h => !actualHeaders.Contains(h)).ToList();

                    if (missingHeaders.Any())
                    {
                        string missing = string.Join(", ", missingHeaders);
                        return new ValidationResult($"Missing required headers: {missing}");
                    }

                    // if (actualHeaders.Length != _requiredHeaders.Length ||
                    //     !_requiredHeaders.Select(h => h.Trim().ToLowerInvariant()).SequenceEqual(actualHeaders))
                    // {
                    //     return new ValidationResult("CSV headers do not match expected format or order.");
                    // }
                }
                else
                {
                    if (!CheckFileSignature(file, _allowedExtensions))
                    {
                        return new ValidationResult("Invalid file");
                    }
                }

                // Read full content and scan for harmful patterns
                using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
                var content = reader.ReadToEnd();

                foreach (var pattern in DangerousPatterns)
                {
                    if (content.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0)
                        return new ValidationResult($"File contains potentially harmful content: '{pattern}'");
                }
            }
            return ValidationResult.Success;
        }
        private bool CheckFileSignature(IFormFile file, string[] _allowedExtensions)
        {
            if (file == null || file.Length == 0)
                return false;

            try
            {
                using (var stream = file.OpenReadStream())
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    reader.BaseStream.Position = 0x0;     // The offset you are reading the data from
                    byte[] data = reader.ReadBytes(0x4);

                    string dataAsHex = BitConverter.ToString(data);

                    var magicBytesMap = MagicBytes();

                    foreach (var fileType in _allowedExtensions)
                    {
                        // Make sure the fileType is in lowercase
                        string lowerFileType = fileType.ToLower();

                        // Check if the fileType exists in MagicBytes and get the corresponding magic signature
                        if (magicBytesMap.ContainsKey(lowerFileType))
                        {
                            var signature = magicBytesMap[lowerFileType];

                            if (signature == dataAsHex)
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                }
            }
            catch
            {
                // Handle exceptions like file access issues
                return false;
            }
        }
        public Dictionary<string, string> MagicBytes()
        {
            return new Dictionary<string, string>
            {
                { ".zip", "00-01-00-00"}, // zip with encryption
                { ".pdf", "25-50-44-46"},             // %PDF
                { ".docx", "50-4B-03-04"},             // ZIP based
                { ".xlsx", "50-4B-03-04"},             // ZIP based
                { ".png", "89-50-4E-47"},             // PNG
                { ".jpg", "FF-D8-FF-E1"},             // JPG
                { ".jpeg", "FF-D8-FF-E0"}             // JPEG
            };
        }

    }
}
