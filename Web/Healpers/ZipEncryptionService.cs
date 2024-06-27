using System.Security.Cryptography;
using System.Text;

namespace Web.Healpers
{
    public class ZipEncryptionService
    {
        string password_mTxtBx = "sdasd";
        public void EncryptFile(string inputFile, string outputFile)
        {
            string password = @"yourPWhere";
            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] key = CreateKey(password);

            string cryptFile = outputFile;
            FileStream fsCrypt = new FileStream(cryptFile+"_en.zip", FileMode.Create);

            RijndaelManaged RMCrypto = new RijndaelManaged();
            byte[] IV = CreateIV(password_mTxtBx);

            CryptoStream cs = new CryptoStream(fsCrypt,
                RMCrypto.CreateEncryptor(key, IV),
                CryptoStreamMode.Write);

            FileStream fsIn = new FileStream(inputFile, FileMode.Open);

            int data;
            while ((data = fsIn.ReadByte()) != -1)
                cs.WriteByte((byte)data);


            fsIn.Close();
            cs.Close();
            fsCrypt.Close();
        }
        public void DecryptFile(string inputFile, string outputFile)
        {
            string password = @"yourPWhere";

            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] key = CreateKey(password);
            FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);
            RijndaelManaged RMCrypto = new RijndaelManaged();
           byte[] IV = CreateIV(password_mTxtBx);

            CryptoStream cs = new CryptoStream(fsCrypt,
                RMCrypto.CreateDecryptor(key, IV),
                CryptoStreamMode.Read);

            FileStream fsOut = new FileStream(outputFile.Remove(outputFile.Length - 4), FileMode.Create);

            int data;
            while ((data = cs.ReadByte()) != -1)
                fsOut.WriteByte((byte)data);

            fsOut.Close();
            cs.Close();
            fsCrypt.Close();

        }
        public static int saltLengthLimit = 32;
        public static byte[] GetSalt(int maximumSaltLength)
        {
            var salt = new byte[maximumSaltLength];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(salt);
            }

            return salt;
        }
        public static byte[] CreateKey(string password)
        {
            var salt = GetSalt(10);

            int iterationCount = 20000; // Nowadays you should use at least 10.000 iterations
            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt, iterationCount))
                return rfc2898DeriveBytes.GetBytes(16);
        }
        public byte[] CreateIV(string password)
        {
            var salt = GetSalt(9);

            const int Iterations = 325;
            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations))
                return rfc2898DeriveBytes.GetBytes(16);
        }
    }
}
