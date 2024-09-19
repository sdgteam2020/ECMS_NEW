using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Web.Healpers
{
    class ZipEncrypt
    {
        // Method to zip a folder
        public static void ZipFolder(string sourceFolder, string zipFile)
        {
            sourceFolder = "I:\\ECMS\\sdgteam2020\\ECMS_NEW\\Web\\wwwroot\\WriteReadData\\ExportAFSACCell";
            ZipFile.CreateFromDirectory(sourceFolder, zipFile, CompressionLevel.Fastest,true);
        }

        // Method to encrypt AES key with RSA public key
        public static byte[] EncryptAesKey(byte[] aesKey, string publicKey)
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);
                return rsa.Encrypt(aesKey, false);
            }
        }

        // Method to encrypt a zip file using AES key
        public static byte[] EncryptZipFile(string zipFilePath, byte[] aesKey, out byte[] aesIv)
        {
            aesIv = new byte[16]; // 128-bit IV
            byte[] encryptedZipFile;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = aesKey;
                aesAlg.GenerateIV();
                aesIv = aesAlg.IV;

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV), CryptoStreamMode.Write))
                    {
                        using (FileStream fsZip = new FileStream(zipFilePath, FileMode.Open))
                        {
                            fsZip.CopyTo(csEncrypt);
                        }
                    }
                    encryptedZipFile = msEncrypt.ToArray();
                }
            }
            return encryptedZipFile;
        }

        // Method to save the encrypted AES key, IV, and encrypted zip file
        public static void SaveEncryptedFile(string outputFile, byte[] encryptedAesKey, byte[] aesIv, byte[] encryptedZipFile)
        {
            using (FileStream fs = new FileStream(outputFile, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(encryptedAesKey.Length); // Write encrypted AES key length
                    bw.Write(encryptedAesKey); // Write encrypted AES key
                    bw.Write(aesIv); // Write AES IV
                    bw.Write(encryptedZipFile); // Write encrypted zip file
                }
            }
        }

        public static void EncryptAndZip(string sourceFolder, string outputEncryptedFile, string publicKey)
        {
            // Zip the folder
            string tempZipFile = "temp.zip";
            ZipFolder(sourceFolder , tempZipFile);

            // Generate AES key
            byte[] aesKey = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(aesKey); // Generate random AES key
            }

            // Encrypt AES key with RSA public key
            byte[] encryptedAesKey = EncryptAesKey(aesKey, publicKey);

            // Encrypt the zip file with AES key
            byte[] aesIv;
            byte[] encryptedZipFile = EncryptZipFile(tempZipFile, aesKey, out aesIv);

            // Save the encrypted AES key, IV, and encrypted zip file to a single output file
            SaveEncryptedFile(outputEncryptedFile, encryptedAesKey, aesIv, encryptedZipFile);

            // Delete the temporary zip file
            File.Delete(tempZipFile);
        }
    }
}
