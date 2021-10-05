using System;
using System.IO;
using System.Linq;
using System.Text;
using Core.Contracts;
using Crypto = System.Security.Cryptography;

namespace Core.Encryption
{
    public class AesEncryptionProvider : IEncryptionProvider
    {
        public AesEncryptionProvider(string key, int keyLength = 32)
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));
            if (string.Empty == key)
                throw new ArgumentException("Key must have have a valid value.", nameof(key));
            if (!validLengths.Contains(keyLength))
                throw new ArgumentException("Invalid key length, key must be of sizes 16, 24 or 32", nameof(key));
            using var hash = new Crypto.SHA512CryptoServiceProvider();
            var hashed = hash.ComputeHash(Encoding.UTF8.GetBytes(key));
            Key = new byte[keyLength];
            Array.ConstrainedCopy(hashed, 0, this.Key, 0, keyLength);
        }

        private static int[] validLengths = new int[] { 16, 24, 32 };
        public AesEncryptionProvider(byte[] key)
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));
            if (!validLengths.Contains(key.Length))
                throw new ArgumentException("Invalid key length, key must be of sizes 16, 24 or 32", nameof(key));
            Key = new byte[key.Length];
            Array.ConstrainedCopy(key, 0, this.Key, 0, key.Length);
        }
        
        public byte[] Key { get; }

        public string Encrypt(string value) => Encrypt(value, Key, null);

        public static string Encrypt(string value, byte[] key, byte[] iv)
        {
            using var aes = Crypto.Aes.Create();
            aes.Key = key;
            if (iv != null) aes.IV = iv;            
            ReadOnlySpan<byte> decodedValue = Encoding.UTF8.GetBytes(value);
            if (aes == null) throw new ArgumentException($"AES Parameter cannot be null.", nameof(aes));
            using var transform = aes.CreateEncryptor(aes.Key, aes.IV);
            using var result = new MemoryStream();
            using (var aesStream = new Crypto.CryptoStream(result, transform, Crypto.CryptoStreamMode.Write))
            {
                using var original = new MemoryStream(decodedValue.ToArray());
                original.CopyTo(aesStream);
            }

            var encrypted = result.ToArray();
            var combined = new byte[aes.IV.Length + encrypted.Length];
            Array.ConstrainedCopy(aes.IV, 0, combined, 0, aes.IV.Length);
            Array.ConstrainedCopy(encrypted, 0, combined, aes.IV.Length, encrypted.Length);
            
            return Convert.ToBase64String(combined);
        }

        public string Decrypt(string encryptedValue)
        {
            return Decrypt(encryptedValue, Key);
        }

        public static string Decrypt(string encryptedValue, byte[] key)
        {
            ReadOnlySpan<byte> combined = Convert.FromBase64String(encryptedValue);
            using var aes = Crypto.Aes.Create();
            if (aes == null) throw new ArgumentException($"AES Parameter cannot be null.", nameof(aes));
            aes.Key = key;
            var iv = combined.Slice(0, aes.IV.Length);
            var encrypted = combined.Slice(aes.IV.Length);
            aes.IV = iv.ToArray();
            using var transform = aes.CreateDecryptor(aes.Key, aes.IV);
            using var result = new MemoryStream();
            using (var aesStream = new Crypto.CryptoStream(result, transform, Crypto.CryptoStreamMode.Write))
            {
                using var original = new MemoryStream(encrypted.ToArray());
                original.CopyTo(aesStream);
            }
            
            return Encoding.UTF8.GetString(result.ToArray());
        }

        public bool TryEncrypt(string value, out string encryptedValue)
        {
            try
            {
                encryptedValue = this.Encrypt(value);
                return true;
            }
            catch (Exception)
            {
                encryptedValue = null;
                return false;
            }
        }

        public bool TryDecrypt(string encryptedValue, out string decryptedValue)
        {
            try
            {
                decryptedValue = this.Decrypt(encryptedValue);
                return true;
            }
            catch (Exception)
            {
                decryptedValue = null;
                return false;
            }
        }
    }
}