using DataTransferObject.Requests;
using DataTransferObject.Response;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using Web.WebHelpers;

namespace Web.Healpers
{
    public class RsaKeyGenerator
    {
        public static DTORsaKeyResponse GenerateKeys()
        {
            
            using (var rsa = RSA.Create(2048))
            {
                var publicKey = Convert.ToBase64String(rsa.ExportSubjectPublicKeyInfo());
                var privateKey = Convert.ToBase64String(rsa.ExportPkcs8PrivateKey());

                DTORsaKeyResponse RsaKeydata = new DTORsaKeyResponse
                {
                    PublicKey = publicKey,
                    PrivateKey = privateKey
                };
                return RsaKeydata;
            }
            return null;
        }
        public static string Decrypt(string encryptedBase64, string privateKeyBase64)
        {
            try
            {
                var privateKeyBytes = Convert.FromBase64String(privateKeyBase64);

                using (var rsa = RSA.Create())
                {
                    rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);

                    var encryptedBytes = Convert.FromBase64String(encryptedBase64);

                    var decryptedBytes = rsa.Decrypt(
                        encryptedBytes,
                        RSAEncryptionPadding.OaepSHA256
                    );

                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            } catch(Exception ex)
            {
                return null;
            }
        }

        public static async Task<T> DecryptRSAWithDTO<T>(string encryptedBase64, string privateKeyBase64)
        {
            try { 
            if (string.IsNullOrEmpty(encryptedBase64))
                return default;

            var privateKeyBytes = Convert.FromBase64String(privateKeyBase64);

            using (var rsa = RSA.Create())
            {
                rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);

                var encryptedBytes = Convert.FromBase64String(encryptedBase64);

                var decryptedBytes = rsa.Decrypt(
                    encryptedBytes,
                    RSAEncryptionPadding.OaepSHA256
                );

                var json = Encoding.UTF8.GetString(decryptedBytes);

                if (string.IsNullOrEmpty(json))
                    return default;

                return await Task.FromResult(JsonConvert.DeserializeObject<T>(json));
            }
            }
            catch (Exception ex)
            {
                return default;
            }
        }

    }
}
