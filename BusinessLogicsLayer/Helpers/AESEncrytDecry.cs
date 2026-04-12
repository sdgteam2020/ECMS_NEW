using DataTransferObject.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusinessLogicsLayer.Helpers
{
    public class AESEncrytDecry
    {
        private static Random RNG = new Random();
        public static string GetSalt()
        {
            var builder = new StringBuilder();
            while (builder.Length < 16)
            {
                builder.Append(RNG.Next(10).ToString());
            }
            return builder.ToString();
        }
        public static string GetKey()
        {
            var builder = new StringBuilder();
            while (builder.Length < 16)
            {
                builder.Append(RNG.Next(10).ToString());
            }
            return builder.ToString();
        }

        public static string DecryptAES(string cipherText, string key)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cipherText) || string.IsNullOrWhiteSpace(key))
                    return null;

                if (key.Length < 16)
                    throw new ArgumentException("Key must be at least 16 characters long.");

                byte[] buffer;

                try
                {
                    buffer = Convert.FromBase64String(cipherText);
                }
                catch
                {
                    // Manipulated Base64
                    return null;
                }

                var iv = Encoding.UTF8.GetBytes(key.Substring(0, 16));
                var keyBytes = Encoding.UTF8.GetBytes(key);

                using (Aes aes = Aes.Create())
                {
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = keyBytes;
                    aes.IV = iv;

                    using (var decryptor = aes.CreateDecryptor())
                    {
                        var result = decryptor.TransformFinalBlock(buffer, 0, buffer.Length);
                        return Encoding.UTF8.GetString(result);
                    }
                }
            }
            catch (CryptographicException)
            {
                // Cipher text manipulated or wrong key
                return null;
            }
            catch
            {
                return null;
            }
        }
        public static async Task<T> DecryptAESWithDTO<T>(string cipherText, string key)
        {
            try
            {
                string json = AESEncrytDecry.DecryptAES(cipherText, key);

                if (string.IsNullOrEmpty(json))
                    return default;

                return await Task.FromResult(JsonConvert.DeserializeObject<T>(json));
            }
            catch
            {
                return default;
            }
        }

    }
}
