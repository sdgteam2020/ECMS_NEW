using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DataTransferObject.Validation
{
    public class SecureFileAttribute : ValidationAttribute
    {
        private readonly string[] _allowedExtensions;
        private readonly string[] _allowedMimeTypes;
        private readonly long _maxFileSize;
        private readonly string[] _expectedHeaders;

        private static readonly byte[][] AllowedSignatures = new byte[][]
        {
        new byte[] { 0xEF, 0xBB, 0xBF }, // UTF-8 BOM
        new byte[] { 0xFF, 0xFE },       // UTF-16 LE
        new byte[] { 0xFE, 0xFF }        // UTF-16 BE
        };

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
            if (file == null)
                return new ValidationResult("File is required.");

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

            byte[] header = new byte[4];
            using (var stream = file.OpenReadStream())
            {
                stream.Read(header, 0, header.Length);
            }

            if (!IsKnownTextSignature(header))
                return new ValidationResult("Invalid file signature.");

            // Read full content and scan for harmful patterns
            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            var content = reader.ReadToEnd();

            foreach (var pattern in DangerousPatterns)
            {
                if (content.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0)
                    return new ValidationResult($"File contains potentially harmful content: '{pattern}'");
            }

            return ValidationResult.Success;
        }

        private bool IsKnownTextSignature(byte[] header)
        {
            foreach (var sig in AllowedSignatures)
            {
                if (header.Take(sig.Length).SequenceEqual(sig))
                    return true;
            }

            return header.All(b => b == 0x0A || b == 0x0D || (b >= 0x20 && b <= 0x7E));
        }

    }
}
