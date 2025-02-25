using System.Security.Cryptography;
using System.Text;

namespace ITStepFinalProject.Utils
{
    public class EncryptionHandler
    {

        private readonly byte[] _key;

        public EncryptionHandler(string key)
        {
            // key = 256 bit.
            _key = HashIt(key);
        }

        public static byte[] HashIt(string key)
        {
            using SHA256 sHA256 = SHA256.Create();
            return sHA256.ComputeHash(Encoding.UTF8.GetBytes(key));
        }

        public string Encrypt(string? text)
        {
            if (text == null || text.Length == 0)
            {
                return "";
            }
            // AES standered. IV is always 16 bytes. 
            using Aes aes = Aes.Create();
            aes.Key = _key;
            aes.GenerateIV();

            using MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(aes.IV, 0, aes.IV.Length);

            using var encryptor = aes.CreateEncryptor();
            using var cryptoStream = new CryptoStream(memoryStream, 
                encryptor, CryptoStreamMode.Write);

            byte[] bytes = Encoding.UTF8.GetBytes(text);

            cryptoStream.Write(bytes, 0, bytes.Length);
            cryptoStream.FlushFinalBlock();

            return Convert.ToBase64String(memoryStream.ToArray());
        }

        public string Decrypt(string? cipherText)
        {
            if (cipherText == null || cipherText.Length == 0)
            {
                return "";
            }
            byte[] combinedBytes = Convert.FromBase64String(cipherText);
            if (combinedBytes.Length < 16)
            {
                throw new ArgumentException("Invalid cipher text length");
            }

            using Aes aes = Aes.Create();
            aes.Key = _key;

            byte[] iv = new byte[16];
            Array.Copy(combinedBytes, 0, iv, 0, 16);
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            // skip the first 16 bytes, because they are the iv
            using var memoryDecrypt = new MemoryStream(combinedBytes, 16, combinedBytes.Length - 16);
            using var cryptoDecrypt = new CryptoStream(memoryDecrypt, decryptor, CryptoStreamMode.Read);
            using var streamReader = new StreamReader(cryptoDecrypt);

            return streamReader.ReadToEnd();
        }
    }
}
