namespace Client.Helpers.Crypto.Interfaces;

public interface ICryptoHelper
{
    byte[] DeriveEncryptionKey(byte[] passwordBytes, byte[] saltBytes);
    byte[] GenerateSalt();
    byte[] Encrypt(byte[] plainTextBytes, byte[] encryptionKeyBytes);
}