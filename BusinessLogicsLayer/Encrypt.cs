using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer
{
    public class Encrypt
    {
        public static string GenerateHash(string input) 
        {
            var crypt = new SHA256Managed();
            string hash = String.Empty;
            byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(input));
            foreach (byte theByte in crypto)
            {
                hash += theByte.ToString("x2");
            }
            return hash;

        }
        public static string GenerateSHA256Hash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("dfgdgdgdgdgdgdgdgdgddgdgdgdgdgdretercdgerervervegr"));
                }

                return sb.ToString();
            }
        }
        public static bool VerifySHA256Hash(string input, string storedHash)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("dfgdgdgdgdgdgdgdgdgddgdgdgdgdgdretercdgerervervegr"));
                }

                string calculatedHash = builder.ToString();

                return storedHash.Equals(calculatedHash, StringComparison.OrdinalIgnoreCase);
            }
        }
        private const string Key = "!QAZ2wsx!@#$1234";
        private const string Iv = "HR$2pIjHR$2pIj12";

        public static string EncryptParameter(string parameter)
        {
            byte[] parameterBytes = Encoding.UTF8.GetBytes(parameter);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Key);
                aes.IV = Encoding.UTF8.GetBytes(Iv);

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                byte[] encryptedBytes = encryptor.TransformFinalBlock(parameterBytes, 0, parameterBytes.Length);

                string encryptedParameter = Convert.ToBase64String(encryptedBytes);

                return Uri.EscapeDataString(encryptedParameter);
            }
        }

        public static string DecryptParameter(string encryptedParameter)
        {
            encryptedParameter = Uri.UnescapeDataString(encryptedParameter);

            byte[] encryptedBytes = Convert.FromBase64String(encryptedParameter);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Key);
                aes.IV = Encoding.UTF8.GetBytes(Iv);

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

                string decryptedParameter = Encoding.UTF8.GetString(decryptedBytes);

                return decryptedParameter;
            }
        }
    }
}
