using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Web.Healpers
{
    class ZipDecrypt
    {   
        // Method to decrypt AES key with RSA private key
        public static byte[] DecryptAesKey(byte[] encryptedAesKey, string privateKey)
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);
                return rsa.Decrypt(encryptedAesKey, false);
            }
        }

        // Method to decrypt the encrypted zip file using AES key
        public static byte[] DecryptZipFile(byte[] encryptedZipFile, byte[] aesKey, byte[] aesIv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = aesKey;
                aesAlg.IV = aesIv;

                using (MemoryStream msDecrypt = new MemoryStream())
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV), CryptoStreamMode.Write))
                    {
                        csDecrypt.Write(encryptedZipFile, 0, encryptedZipFile.Length);
                    }
                    return msDecrypt.ToArray();
                }
            }
        }

        // Method to read the encrypted file and extract AES key, IV, and encrypted zip file
        public static (byte[] encryptedAesKey, byte[] aesIv, byte[] encryptedZipFile) ReadEncryptedFile(string encryptedFile)
        {
            using (FileStream fs = new FileStream(encryptedFile, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    int aesKeyLength = br.ReadInt32(); // Read encrypted AES key length
                    byte[] encryptedAesKey = br.ReadBytes(aesKeyLength); // Read encrypted AES key
                    byte[] aesIv = br.ReadBytes(16); // Read AES IV (16 bytes)
                    byte[] encryptedZipFile = br.ReadBytes((int)(fs.Length - fs.Position)); // Read encrypted zip file
                    return (encryptedAesKey, aesIv, encryptedZipFile);
                }
            }
        }

        // Method to save decrypted zip file and unzip
        public static void SaveAndUnzip(string decryptedZipFile, byte[] zipData, string outputFolder)
        {
            File.WriteAllBytes(decryptedZipFile, zipData);
            ZipFile.ExtractToDirectory(decryptedZipFile, outputFolder);
            File.Delete(decryptedZipFile); // Delete decrypted zip file after unzipping
        }

        public static void DecryptAndUnzip(string encryptedFile, string outputFolder, string privateKey)
        {
            // Read encrypted file and extract AES key, IV, and encrypted zip file
            var (encryptedAesKey, aesIv, encryptedZipFile) = ReadEncryptedFile(encryptedFile);

            // Decrypt AES key with RSA private key
            byte[] aesKey = DecryptAesKey(encryptedAesKey, privateKey);

            // Decrypt the zip file using AES key
            byte[] zipData = DecryptZipFile(encryptedZipFile, aesKey, aesIv);

            // Save the decrypted zip file and unzip
            SaveAndUnzip("decrypted.zip", zipData, outputFolder);
        }
    }
}
