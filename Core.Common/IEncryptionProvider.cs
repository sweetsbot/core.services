namespace Core.Common
{
    public interface IEncryptionProvider
    {
        string Encrypt(string value);
        string Decrypt(string encryptedValue);
    }
}