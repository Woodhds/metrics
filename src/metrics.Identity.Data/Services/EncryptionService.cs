using System;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace metrics.Identity.Data.Services
{
    public interface IEncryptionService
    {
        string Encrypt(string cipherText);
        string Decrypt(string value);
    }

    public class EncryptionService : IEncryptionService
    {
        private readonly IOptions<EncryptionOptions> _options;

        public EncryptionService(IOptions<EncryptionOptions> options)
        {
            _options = options;
        }

        public string Encrypt(string cipherText)
        {
            using var aes = Aes.Create();
            using var encryptor = aes.CreateEncryptor(Convert.FromBase64String(_options.Value.EncryptKey),
                Convert.FromBase64String(_options.Value.DecryptKey));

            byte[] buffer;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(cipherText);
                    }

                    buffer = ms.ToArray();
                }
            }

            return Convert.ToBase64String(buffer);
        }


        public string Decrypt(string value)
        {
            using var aes = Aes.Create();
            using var decryptor = aes.CreateDecryptor(Convert.FromBase64String(_options.Value.EncryptKey),
                Convert.FromBase64String(_options.Value.DecryptKey));

            var base64Bytes = Convert.FromBase64String(value);
            using var ms = new MemoryStream(base64Bytes);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sw = new StreamReader(cs);

            return sw.ReadToEnd();
        }
    }
}