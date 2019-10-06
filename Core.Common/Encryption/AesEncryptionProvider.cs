using System;
using System.IO;
using System.Text;
using Core.Common;
using Crypto = System.Security.Cryptography;

namespace Core.Encryption
{
    public class AesEncryptionProvider : IEncryptionProvider
    {
        private readonly byte[] _aesKey;

        public AesEncryptionProvider(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key must have have a valid value.", nameof(key));
            }

            _aesKey = new byte[24];
            using (var hash = new Crypto.SHA512CryptoServiceProvider())
            {
                var hashed = hash.ComputeHash(Encoding.UTF8.GetBytes(key ?? ""));
                Array.ConstrainedCopy(hashed, 0, this._aesKey, 0, 24);
            }
        }

        public string Encrypt(string value)
        {
            ReadOnlySpan<byte> decodedValue = Encoding.UTF8.GetBytes(value);
            using (var aes = Crypto.Aes.Create())
            {
                if (aes == null) throw new ArgumentException($"AES Parameter cannot be null.", nameof(aes));
                aes.Key = _aesKey;
                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var result = new MemoryStream())
                {
                    using (var aesStream = new Crypto.CryptoStream(result, encryptor, Crypto.CryptoStreamMode.Write))
                    using (var original = new MemoryStream(decodedValue.ToArray()))
                    {
                        original.CopyTo(aesStream);
                    }

                    var encrypted = result.ToArray();
                    var combined = new byte[aes.IV.Length + encrypted.Length];
                    Array.ConstrainedCopy(aes.IV, 0, combined, 0, aes.IV.Length);
                    Array.ConstrainedCopy(encrypted, 0, combined, aes.IV.Length, encrypted.Length);

                    return Convert.ToBase64String(combined);
                }
            }
        }

        public string Decrypt(string encryptedValue)
        {
            ReadOnlySpan<byte> combined = Convert.FromBase64String(encryptedValue);
            using (var aes = Crypto.Aes.Create())
            {
                if (aes == null) throw new ArgumentException($"AES Parameter cannot be null.", nameof(aes));
                aes.Key = _aesKey;
                var iv = combined.Slice(0, aes.IV.Length);
                var encrypted = combined.Slice(aes.IV.Length);
                aes.IV = iv.ToArray();
                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var result = new MemoryStream())
                using (var aesStream = new Crypto.CryptoStream(result, decryptor, Crypto.CryptoStreamMode.Write))
                using (var original = new MemoryStream(encrypted.ToArray()))
                {
                    original.CopyTo(aesStream);

                    return Encoding.UTF8.GetString(result.ToArray());
                }
            }
        }
    }
}