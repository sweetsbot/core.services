namespace Core.Contracts
{
    public interface IEncryptionProvider
    {
        byte[] Key { get; }
        string Encrypt(string value);
        string Decrypt(string encryptedValue);
        bool TryEncrypt(string value, out string encryptedValue);
        bool TryDecrypt(string encryptedValue, out string decryptedValue);
    }
}