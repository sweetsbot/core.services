using System;
using System.Security.Cryptography;
using System.Text;
using Xunit;
using Core.Contracts;
using Core.Encryption;

namespace Core.Tests
{
    public class AesEncryptionProviderTests
    {
        #region Ctor
        [Theory]
        [InlineData("secretkey", "ho8LEsB+9jB9ByG9eFEDHr1r6UQjdgOG")]
        [InlineData("magickey", "9cgRiXzwpv5J2VISsT0PWnBw/vPc3lZQ")]
        [InlineData("this is a key   ", "bysoZm6gIexsM0icl/vCwsaaCSKXjBWC")]
        public void Ctor_KeyValid(string key, string expectedB64)
        {
            const int keyLength = 24;
            // arrange
            byte[] expected = Convert.FromBase64String(expectedB64);

            // act
            var provider = new AesEncryptionProvider(key);

            // assert
            Assert.Equal(expected, provider.Key);
            Assert.Equal(keyLength, expected.Length);
            Assert.Equal(keyLength, provider.Key.Length);
        }

        [Fact]
        public void Ctor_EmptyKeyThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new AesEncryptionProvider(new byte[0]));
            Assert.Throws<ArgumentException>(() => new AesEncryptionProvider(string.Empty));
        }

        [Fact]
        public void Ctor_NullKeyThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new AesEncryptionProvider(null));
            Assert.Throws<ArgumentNullException>(() => new AesEncryptionProvider(null, 24));
        }
        #endregion
        
        #region Encrypt
        [Theory]
        [InlineData("Hello there.", "ho8LEsB+9jB9ByG9eFEDHr1r6UQjdgOG", "TQ36HLyQYwO0bYf5YFXMSw==", "bZtq3qI+hm7w9qtbijBXmQ==")]
        [InlineData("Hello there", "ho8LEsB+9jB9ByG9eFEDHr1r6UQjdgOG", "HAyLlVa32DoTiOO1b3h8Pw==", "s+6CYq3CLt3Xzrt9eMK6Vw==")]
        [InlineData("Secret Passcode", "ho8LEsB+9jB9ByG9eFEDHr1r6UQjdgOG", "TQ36HLyQYwO0bYf5YFXMSw==", "ktDzJfJonnR0FCUIMinWyQ==")]
        [InlineData("This is sensitive information", "ho8LEsB+9jB9ByG9eFEDHr1r6UQjdgOG", "TQ36HLyQYwO0bYf5YFXMSw==", "bKA7BI7K9s4raqNcou/BjBAsHT5fJ+Hup2npl8ESC/8=")]
        public void Encrypt_StaticEncryptValuesSuccess(string value, string keyB64, string ivB64, string expectedB64)
        {
            // arrange
            var key = Convert.FromBase64String(keyB64);
            var iv = Convert.FromBase64String(ivB64);
            var asBytes = Convert.FromBase64String(expectedB64);
            var expectedBytes = new byte[iv.Length + asBytes.Length];
            Array.ConstrainedCopy(iv, 0, expectedBytes, 0, iv.Length);
            Array.ConstrainedCopy(asBytes, 0, expectedBytes, iv.Length, asBytes.Length);
            var expected = Convert.ToBase64String(expectedBytes);

            // act
            var actual = AesEncryptionProvider.Encrypt(value, key, iv);

            // assert
            Assert.Equal(expected, actual);
            Assert.True(actual.Length > 16);
            Assert.True(IsBase64String(actual));
        }

        [Fact]
        public void Encrypt_StaticEncryptIVPrependedInReturn()
        {
            byte[] key = Convert.FromBase64String("ho8LEsB+9jB9ByG9eFEDHr1r6UQjdgOG");
            byte[] iv = Convert.FromBase64String("TQ36HLyQYwO0bYf5YFXMSw==");

            var result = AesEncryptionProvider.Encrypt("secret value", key, iv);
            byte[] ivFromResult = Convert.FromBase64String(result)[..16];

            Assert.Equal(iv, ivFromResult);
        }

        [Fact]
        public void Encrypt_StaticEncryptReturnsBase64String()
        {
            byte[] key = Convert.FromBase64String("ho8LEsB+9jB9ByG9eFEDHr1r6UQjdgOG");
            byte[] iv = Convert.FromBase64String("TQ36HLyQYwO0bYf5YFXMSw==");

            var actual = AesEncryptionProvider.Encrypt("secret value", key, iv);
            Assert.True(IsBase64String(actual));
        }

        private bool IsBase64String(string value)
        {
            var buffer = new Span<byte>(new byte[value.Length]);
            return Convert.TryFromBase64String(value, buffer, out _);
        }

        [Fact]
        public void Encrypt_StaticEncryptNullKeyThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => AesEncryptionProvider.Encrypt("Safe Value", null, null));
        }

        [Fact]
        public void Encrypt_StaticEncryptInvalidLengthKeyThrowsException()
        {
            Assert.Throws<CryptographicException>(() => AesEncryptionProvider.Encrypt("Safe Value", new byte[] {120}, null));
            Assert.Throws<CryptographicException>(() => AesEncryptionProvider.Encrypt("Safe Value", new byte[0], null));
        }

        [Fact]
        public void Encrypt_StaticEncryptIVCanBeNull()
        {
            byte[] key = Convert.FromBase64String("dGhpcyBpcyBhIHZhbGlkIQ==");
            var result = AesEncryptionProvider.Encrypt("Safe Value", key, null);

            Assert.NotEmpty(result);
            Assert.True(result.Length > 16);
        }

        [Fact]
        public void Encrypt_StaticEncryptValidKeySuccess()
        {
            byte[] key = Convert.FromBase64String("dGhpcyBpcyBhIHZhbGlkIQ==");
            var result = AesEncryptionProvider.Encrypt("Safe Value", key, null);

            Assert.NotEmpty(result);
            Assert.True(result.Length > 16);
        }
        #endregion

        #region Decrypt

        

        #endregion
    }
}