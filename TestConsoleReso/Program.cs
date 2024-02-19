using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


class Program
{
    public class AesOperation
    {
        public static string Encrypt(string key, string data)
        {
            Encoding unicode = Encoding.Unicode;

            return Convert.ToBase64String(Encrypt(unicode.GetBytes(key), unicode.GetBytes(data)));
        }

        public static string Decrypt(string key, string data)
        {
            Encoding unicode = Encoding.Unicode;

            return unicode.GetString(Encrypt(unicode.GetBytes(key), Convert.FromBase64String(data)));
        }
        public static byte[] Encrypt(byte[] key, byte[] data)
        {
            return EncryptOutput(key, data).ToArray();
        }

        public static byte[] Decrypt(byte[] key, byte[] data)
        {
            return EncryptOutput(key, data).ToArray();
        }
        private static byte[] EncryptInitalize(byte[] key)
        {
            byte[] s = Enumerable.Range(0, 256)
              .Select(i => (byte)i)
              .ToArray();

            for (int i = 0, j = 0; i < 256; i++)
            {
                j = (j + key[i % key.Length] + s[i]) & 255;

                Swap(s, i, j);
            }

            return s;
        }

        private static IEnumerable<byte> EncryptOutput(byte[] key, IEnumerable<byte> data)
        {
            byte[] s = EncryptInitalize(key);

            int i = 0;
            int j = 0;

            return data.Select((b) =>
            {
                i = (i + 1) & 255;
                j = (j + s[i]) & 255;

                Swap(s, i, j);

                return (byte)(b ^ s[(s[i] + s[j]) & 255]);
            });
        }
        private static void Swap(byte[] s, int i, int j)
        {
            byte c = s[i];

            s[i] = s[j];
            s[j] = c;
        }
    }
    static void Main()
    {

        var key = "b14ca5898a4e4133bbce2ea2315a1916";

        //Console.WriteLine("Please enter a secret key for the symmetric algorithm.");
        //var key = Console.ReadLine();

        Console.WriteLine("Please enter a string for encryption");
        var str = Console.ReadLine();
        var encryptedString = AesOperation.Encrypt(key, str);
        Console.WriteLine($"encrypted string = {encryptedString}");

        var decryptedString = AesOperation.Decrypt(key, encryptedString);
        Console.WriteLine($"decrypted string = {decryptedString}");

        Console.ReadKey();

    }
}
