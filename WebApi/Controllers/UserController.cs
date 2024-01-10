using BusinessLogicsLayer;
using DataTransferObject.Response.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        //private readonly IUnitOfWork unitOfWork;

        //public UserController(IUnitOfWork unitOfWork)
        //{
        //    this.unitOfWork = unitOfWork;
        //}
        public class Icno
        {
            public string IcNo { get; set; }
            public byte[] SHA256 { get; set; }
            public string EncryptString { get; set; }
            public byte[] iv { get; set; }
        }

        [HttpPost]
        public IActionResult Post(string IcNo)
        {

            helpers helpers = new helpers();
            // Create sha256 hash
            //SHA256 mySHA256 = SHA256Managed.Create();
            //byte[] key =mySHA256.ComputeHash(Encoding.ASCII.GetBytes("1234"));
            //byte[] iv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
            //  
            //string encrypted = helpers.EncryptString(IcNo, key, iv);
            //string decrypted = helpers.DecryptString(encrypted, key, iv);
          
            var data = Encoding.UTF8.GetBytes(IcNo);
            var key = Encoding.ASCII.GetBytes("HZkbzPz7sSK995LTGrJdOg==");//helpers.GenerateAESKey();
            var encryptedDataAsString = helpers.Encrypt(data, key, out var iv);
            //var encryptedDataAsString = Convert.ToHexString(encryptedData);
            //Console.WriteLine("Encrypted Value:\n" + encryptedDataAsString);


            return Ok(key);
        }
        private string HashPassword(string password, string salt)
        {
            string hashingAlgorithm = "HMACSHA256";
            byte[] passwordBytes = Encoding.Unicode.GetBytes(password);
            byte[] saltBytes = Convert.FromBase64String(salt);
            var saltyPasswordBytes = new byte[saltBytes.Length + passwordBytes.Length];

            Buffer.BlockCopy(saltBytes, 0, saltyPasswordBytes, 0, saltBytes.Length);
            Buffer.BlockCopy(passwordBytes, 0, saltyPasswordBytes, saltBytes.Length, passwordBytes.Length);

            switch (hashingAlgorithm)
            {
                case "HMACSHA256":
                    return Convert.ToBase64String(new HMACSHA256(saltBytes).ComputeHash(saltyPasswordBytes));
                default:
                    // Supported types include: SHA1, MD5, SHA256, SHA384, SHA512
                    HashAlgorithm algorithm = HashAlgorithm.Create(hashingAlgorithm);

                    if (algorithm != null)
                    {
                        return Convert.ToBase64String(algorithm.ComputeHash(saltyPasswordBytes));
                    }

                    throw new CryptographicException("Unknown hash algorithm");
            }
        }
       
        public static string GenerateSalt()
        {
            using var rng = RandomNumberGenerator.Create();
            var byteSalt = new byte[16];
            rng.GetBytes(byteSalt);
            var salt = Convert.ToBase64String(byteSalt);
            return salt;
        }
       
    }
}
