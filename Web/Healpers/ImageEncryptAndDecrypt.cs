using BusinessLogicsLayer.EncryptionSetting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Web.Healpers.BaseInterfaces;

namespace Web.Healpers
{
    public class ImageEncryptAndDecrypt : IImageEncryptAndDecrypt
    {
        private static readonly Regex s_dataUri = new(@"^data:(?<mime>[^;]+);base64,(?<data>.+)$",RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly IEncryptionSettingBL encryptionSettingBL;

        #region Key and IV Initialization
        //// Fixed key and IV (hardcoded)
        //private static readonly byte[] FixedKey = new byte[32] // 256-bit key
        //{
        //    0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF,
        //    0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF
        //};

        //private static readonly byte[] FixedIV = new byte[16] // 128-bit IV
        //{
        //    0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF
        //};
        #endregion
        public ImageEncryptAndDecrypt(IEncryptionSettingBL encryptionSettingBL)
        {
            this.encryptionSettingBL = encryptionSettingBL;
        }
        public async Task EncryptImageFile(string inputFilePath, string encryptedFilePath)
        {
            using (Aes aes = Aes.Create())
            {
                // Await the asynchronous Get method to retrieve the key record
                var keyRecord = await encryptionSettingBL.Get(1);
                if (keyRecord != null)
                {
                    aes.Key = keyRecord.KeyBytes;
                    aes.IV = keyRecord.IVBytes;
                }
                else
                {
                   throw new InvalidOperationException("Encryption key record not found.");
                }


                // Create an encryptor
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                // Open the input file to read the original data
                using (FileStream inputFileStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
                {
                    // Open the output file to write encrypted data
                    using (FileStream outputFileStream = new FileStream(encryptedFilePath, FileMode.Create, FileAccess.Write))
                    {
                        // Create a CryptoStream to encrypt data as it is read
                        using (CryptoStream cryptoStream = new CryptoStream(outputFileStream, encryptor, CryptoStreamMode.Write))
                        {
                            // Read the input file in chunks to save memory
                            byte[] buffer = new byte[4096];
                            int bytesRead;
                            while ((bytesRead = inputFileStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                cryptoStream.Write(buffer, 0, bytesRead);
                            }
                        }
                    }
                }
            }
        }
        public async Task DecryptImageFile(string encryptedFilePath, string decryptedFilePath)
        {
            using (Aes aes = Aes.Create())
            {
                // Await the asynchronous Get method to retrieve the key record
                var keyRecord = await encryptionSettingBL.Get(1);
                if (keyRecord != null)
                {
                    aes.Key = keyRecord.KeyBytes;
                    aes.IV = keyRecord.IVBytes;
                }
                else
                {
                    throw new InvalidOperationException("Encryption key record not found.");
                }

                // Create a decryptor
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                // Open the encrypted file to read encrypted data
                using (FileStream encryptedFileStream = new FileStream(encryptedFilePath, FileMode.Open, FileAccess.Read))
                {
                    // Open the output file to write decrypted data
                    using (FileStream decryptedFileStream = new FileStream(decryptedFilePath, FileMode.Create, FileAccess.Write))
                    {
                        // Create a CryptoStream to decrypt data as it is read
                        using (CryptoStream cryptoStream = new CryptoStream(decryptedFileStream, decryptor, CryptoStreamMode.Write))
                        {
                            // Read the encrypted file in chunks to save memory
                            byte[] buffer = new byte[4096];
                            int bytesRead;
                            while ((bytesRead = encryptedFileStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                cryptoStream.Write(buffer, 0, bytesRead);
                            }
                        }
                    }
                }
            }
        }
        public async Task<IFormFile> DecryptImageToIFormFile(string encryptedFilePath, string fileName)
        {
            using (Aes aes = Aes.Create())
            {
                // Await the asynchronous Get method to retrieve the key record
                var keyRecord = await encryptionSettingBL.Get(1);
                if (keyRecord != null)
                {
                    aes.Key = keyRecord.KeyBytes;
                    aes.IV = keyRecord.IVBytes;
                }
                else
                {
                    throw new InvalidOperationException("Encryption key record not found.");
                }

                // Ensure the encrypted file exists
                if (!File.Exists(encryptedFilePath))
                {
                    throw new FileNotFoundException($"The file {encryptedFilePath} does not exist.");
                }

                // Create a decryptor
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (FileStream encryptedFileStream = new FileStream(encryptedFilePath, FileMode.Open, FileAccess.Read))
                {
                    // Create a new MemoryStream that we won't dispose
                    var memoryStream = new MemoryStream();

                    // Decrypt and write to memoryStream
                    CryptoStream cryptoStream = null;

                    try
                    {
                        cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write);

                        byte[] buffer = new byte[8192]; // Buffer size 8 KB
                        int bytesRead;

                        while ((bytesRead = encryptedFileStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            cryptoStream.Write(buffer, 0, bytesRead);
                        }

                        // Finalize the decryption process
                        cryptoStream.FlushFinalBlock();

                        // Reset memoryStream position for reading
                        memoryStream.Position = 0;

                        // Validate decrypted data
                        if (memoryStream.Length == 0)
                        {
                            throw new InvalidOperationException("Decryption resulted in an empty file.");
                        }

                        // Return IFormFile from memoryStream
                        return new FormFile(memoryStream, 0, memoryStream.Length, "file", fileName);
                    }
                    catch (Exception)
                    {
                        // Dispose of streams in case of an error
                        memoryStream?.Dispose();
                        cryptoStream?.Dispose();
                        throw;
                    }
                }
            }
        }
        public async Task<string> DecryptImageToBase64(string encryptedFilePath)
        {
            // Create a provider for mapping extensions to MIME types
            var provider = new FileExtensionContentTypeProvider();

            string temp = encryptedFilePath.Replace(".enc", string.Empty);

            // Determine the MIME type based on the file extension
            string contentType;
            if (!provider.TryGetContentType(temp, out contentType))
            {
                contentType = "application/octet-stream"; // Default to binary if no mapping exists
            }

            using (var inputStream = new FileStream(encryptedFilePath, FileMode.Open, FileAccess.Read))
            using (var aes = Aes.Create())
            {
                // Await the asynchronous Get method to retrieve the key record
                var keyRecord = await encryptionSettingBL.Get(1);
                if (keyRecord != null)
                {
                    aes.Key = keyRecord.KeyBytes;
                    aes.IV = keyRecord.IVBytes;
                }
                else
                {
                    throw new InvalidOperationException("Encryption key record not found.");
                }

                using (var cryptoStream = new CryptoStream(inputStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                using (var memoryStream = new MemoryStream())
                {
                    cryptoStream.CopyTo(memoryStream);
                    byte[] decryptedBytes = memoryStream.ToArray();
                    
                    // Add the content type as a prefix
                    return $"data:{contentType};base64,{Convert.ToBase64String(decryptedBytes)}";
                }
            }
        }
        [SupportedOSPlatform("windows")]
        public string CompressBase64(string base64,int maxWidth = 1024,long jpegQuality = 75,bool returnDataUri = true)
        {
            if (string.IsNullOrWhiteSpace(base64))
                throw new ArgumentException("Base64 input is null or empty.", nameof(base64));

            // Strip data: header if present
            string? mime = null;
            string raw = base64;
            var m = s_dataUri.Match(base64);
            if (m.Success)
            {
                mime = m.Groups["mime"].Value;
                raw = m.Groups["data"].Value;
            }

            if (!TryFromBase64String(raw, out var inputBytes))
                throw new ArgumentException("Input is not valid Base64.", nameof(base64));

            using var input = new MemoryStream(inputBytes, writable: false);

            // Validate image stream (throws on invalid)
            using var src = Image.FromStream(input, useEmbeddedColorManagement: false, validateImageData: true);

            // Strip metadata (minor size win; ignore failures for non-existent items)
            foreach (var id in src.PropertyIdList)
            {
                try { src.RemovePropertyItem(id); } catch { /* ignore */ }
            }

            using var work = ResizeIfNeeded(src, maxWidth);

            bool hasAlpha =
                work.PixelFormat.HasFlag(PixelFormat.Alpha) ||
                work.PixelFormat.HasFlag(PixelFormat.PAlpha) ||
                (mime is not null && mime.Contains("png", StringComparison.OrdinalIgnoreCase));

            using var output = new MemoryStream();

            if (hasAlpha)
            {
                work.Save(output, ImageFormat.Png);
                var b64 = Convert.ToBase64String(output.ToArray());
                return returnDataUri ? $"data:image/png;base64,{b64}" : b64;
            }
            else
            {
                var enc = GetEncoder(ImageFormat.Jpeg) ?? throw new InvalidOperationException("JPEG encoder not found.");
                using var encParams = new EncoderParameters(1);
                encParams.Param[0] = new EncoderParameter(Encoder.Quality, Clamp(jpegQuality, 0L, 100L));
                work.Save(output, enc, encParams);

                var b64 = Convert.ToBase64String(output.ToArray());
                return returnDataUri ? $"data:image/jpeg;base64,{b64}" : b64;
            }
        }

        [SupportedOSPlatform("windows")]
        private static Image ResizeIfNeeded(Image src, int maxWidth)
        {
            if (maxWidth <= 0 || src.Width <= maxWidth)
                return (Image)src.Clone();

            int newW = maxWidth;
            int newH = (int)Math.Round(src.Height * (newW / (double)src.Width));

            // 32bppArgb avoids indexed/palette surprises and preserves alpha if present.
            var dest = new Bitmap(newW, newH, PixelFormat.Format32bppArgb);

            using (var g = Graphics.FromImage(dest))
            {
                g.CompositingMode = CompositingMode.SourceCopy;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                var rect = new Rectangle(0, 0, newW, newH);
                g.DrawImage(src, rect, 0, 0, src.Width, src.Height, GraphicsUnit.Pixel);
            }

            return dest;
        }

        [SupportedOSPlatform("windows")]
        private static ImageCodecInfo? GetEncoder(ImageFormat format)
        {
            var encoders = ImageCodecInfo.GetImageDecoders();
            for (int i = 0; i < encoders.Length; i++)
            {
                if (encoders[i].FormatID == format.Guid)
                    return encoders[i];
            }
            return null;
        }

        private static long Clamp(long value, long min, long max)
            => value < min ? min : (value > max ? max : value);

        // Avoid throwing FormatException for analyzers that prefer Try* patterns.
        private static bool TryFromBase64String(string s, [NotNullWhen(true)] out byte[]? bytes)
        {
            try
            {
                bytes = Convert.FromBase64String(s);
                return true;
            }
            catch
            {
                bytes = null;
                return false;
            }
        }
    }
}
