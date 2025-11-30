using System.Security.Cryptography;
using System.Text;

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
            var iv = Encoding.UTF8.GetBytes(key.Substring(0, 16));
            var keyBytes = Encoding.UTF8.GetBytes(key);

            var buffer = Convert.FromBase64String(cipherText);

            using Aes aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = keyBytes;
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            var result = decryptor.TransformFinalBlock(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(result);
        }
    }
}
