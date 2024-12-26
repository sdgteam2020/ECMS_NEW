using Microsoft.AspNetCore.StaticFiles;
using System.Security.Cryptography;

namespace Web.Healpers
{
    public static class ImageEncryptAndDecrypt
    {
        // Fixed key and IV (hardcoded)
        private static readonly byte[] FixedKey = new byte[32] // 256-bit key
        {
            0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF,
            0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF
        };

        private static readonly byte[] FixedIV = new byte[16] // 128-bit IV
        {
            0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF
        };
        public static void EncryptImageFile(string inputFilePath, string encryptedFilePath)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = FixedKey;
                aes.IV = FixedIV;

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
        public static void DecryptImageFile(string encryptedFilePath, string decryptedFilePath)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = FixedKey;
                aes.IV = FixedIV;

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
        public static IFormFile DecryptImageToIFormFile(string encryptedFilePath, string fileName)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = FixedKey;
                aes.IV = FixedIV;

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

        public static string DecryptImageToBase64(string encryptedFilePath)
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
                aes.Key = FixedKey;
                aes.IV = FixedIV;

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
    }
}
