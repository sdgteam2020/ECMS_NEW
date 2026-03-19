using System.Security.Cryptography;
using System.Text;

namespace Web.Healpers
{
    public class ApiHelpers
    {
        public string EncDec(string text, string PubKey, string PrvKey, Boolean truefalse)
        {
            var SharedKey = PrvKey + PubKey;
            var result = text;
            try
            {
                if (truefalse)//if true then encrypt else decrypt
                {
                    SHA256 mySHA256 = SHA256Managed.Create();
                    byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(SharedKey));
                    byte[] iv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
                    if (string.IsNullOrEmpty(result))
                        result = EncryptString(result, key, iv);
                    //string decrypted = helpers.DecryptString(encrypted, key, iv);

                    return result;
                }
                else
                {
                    SHA256 mySHA256 = SHA256Managed.Create();
                    byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(SharedKey));
                    byte[] iv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
                    if (!string.IsNullOrEmpty(result))
                        result = DecryptString(text, key, iv);

                    return result;
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }
        public static byte[] Encrypt(byte[] data, byte[] key, out byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC; // better security
                aes.Key = key;
                aes.GenerateIV(); // IV = Initialization Vector
                using (var encryptor = aes.CreateEncryptor())
                {
                    iv = aes.IV;
                    return encryptor.TransformFinalBlock(data, 0, data.Length);
                }
            }
        }
        public static byte[] Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC; // same as for encryption
                using (var decryptor = aes.CreateDecryptor())
                {
                    return decryptor.TransformFinalBlock(data, 0, data.Length);
                }
            }
        }
        public static byte[] GenerateAESKey()
        {
            var rnd = new RNGCryptoServiceProvider();
            var b = new byte[16];
            rnd.GetNonZeroBytes(b);
            return b;
        }

        public string EncryptString(string plainText, byte[] key, byte[] iv)
        {
            // Instantiate a new Aes object to perform string symmetric encryption
            Aes encryptor = Aes.Create();

            encryptor.Mode = CipherMode.CBC;
            //encryptor.KeySize = 256;
            //encryptor.BlockSize = 128;
            //encryptor.Padding = PaddingMode.Zeros;

            // Set key and IV
            encryptor.Key = key;
            encryptor.IV = iv;

            // Instantiate a new MemoryStream object to contain the encrypted bytes
            MemoryStream memoryStream = new MemoryStream();

            // Instantiate a new encryptor from our Aes object
            ICryptoTransform aesEncryptor = encryptor.CreateEncryptor();

            // Instantiate a new CryptoStream object to process the data and write it to the 
            // memory stream
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write);

            // Convert the plainText string into a byte array
            byte[] plainBytes = Encoding.ASCII.GetBytes(plainText);

            // Encrypt the input plaintext string
            cryptoStream.Write(plainBytes, 0, plainBytes.Length);

            // Complete the encryption process
            cryptoStream.FlushFinalBlock();

            // Convert the encrypted data from a MemoryStream to a byte array
            byte[] cipherBytes = memoryStream.ToArray();

            // Close both the MemoryStream and the CryptoStream
            memoryStream.Close();
            cryptoStream.Close();

            // Convert the encrypted byte array to a base64 encoded string
            string cipherText = Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);

            // Return the encrypted data as a string
            return cipherText;
        }

        public string DecryptString(string cipherText, byte[] key, byte[] iv)
        {
            // Instantiate a new Aes object to perform string symmetric encryption
            Aes encryptor = Aes.Create();

            encryptor.Mode = CipherMode.CBC;
            //encryptor.KeySize = 256;
            //encryptor.BlockSize = 128;
            //encryptor.Padding = PaddingMode.Zeros;

            // Set key and IV
            encryptor.Key = key;
            encryptor.IV = iv;

            // Instantiate a new MemoryStream object to contain the encrypted bytes
            MemoryStream memoryStream = new MemoryStream();

            // Instantiate a new encryptor from our Aes object
            ICryptoTransform aesDecryptor = encryptor.CreateDecryptor();

            // Instantiate a new CryptoStream object to process the data and write it to the 
            // memory stream
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);

            // Will contain decrypted plaintext
            string plainText = String.Empty;

            try
            {
                // Convert the ciphertext string into a byte array
                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                // Decrypt the input ciphertext string
                cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);

                // Complete the decryption process
                cryptoStream.FlushFinalBlock();

                // Convert the decrypted data from a MemoryStream to a byte array
                byte[] plainBytes = memoryStream.ToArray();

                // Convert the decrypted byte array to string
                plainText = Encoding.ASCII.GetString(plainBytes, 0, plainBytes.Length);
            }
            finally
            {
                // Close both the MemoryStream and the CryptoStream
                memoryStream.Close();
                cryptoStream.Close();
            }

            // Return the decrypted data as a string
            return plainText;
        }
        /// <summary>
        /// Methot to Get Hash value 
        /// </summary>
        /// <param name="plainText">Plain Text</param>
        /// <returns>Hashed value</returns>
        public string GetHashValue(string plainText)
        {
            string returnTxt = plainText.Trim();
            returnTxt = (returnTxt + " - 1504170979800609").ToUpper();

            byte[] firstHash;
            using (var sha256 = SHA256.Create())
            {
                firstHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(returnTxt));
            }

            returnTxt = Convert.ToHexString(firstHash).ToUpperInvariant(); 
            returnTxt = (returnTxt + "**42314925074701##").ToUpper();

            byte[] secondHash;
            using (var sha256 = SHA256.Create())
            {
                secondHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(returnTxt));
            }

            return Convert.ToHexString(secondHash).ToUpperInvariant();
        }
    }
}