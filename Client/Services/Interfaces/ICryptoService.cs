namespace Client.Services.Interfaces;

public interface ICryptoService
{
    byte[] DeriveEncryptionKey(byte[] passwordBytes, byte[] saltBytes);
    public byte[] GenerateRandomEncryptionKey();
    byte[] GenerateSalt();
    byte[] Encrypt(byte[] plainTextBytes, byte[] encryptionKeyBytes);
    byte[] Decrypt(byte[] encryptedTextBytes, byte[] encryptionKeyBytes);
}