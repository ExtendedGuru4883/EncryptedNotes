namespace Client.Helpers.Crypto.Interfaces;

public interface ICryptoHelper
{
    byte[] DeriveEncryptionKey(byte[] passwordBytes, byte[] saltBytes, uint keyLength);
    byte[] GenerateSalt (uint length);
}