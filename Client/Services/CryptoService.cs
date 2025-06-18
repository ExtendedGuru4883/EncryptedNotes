using System.Runtime.Versioning;
using BlazorSodium.Sodium;
using Client.Services.Interfaces;

namespace Client.Services;

[SupportedOSPlatform("browser")]
public class CryptoService : ICryptoService
{
    public byte[] DeriveEncryptionKey(byte[] passwordBytes, byte[] saltBytes)
    {
        return PasswordHash.Crypto_PwHash(
            SecretBox.KEY_BYTES,
            passwordBytes,
            saltBytes,
            PasswordHash.OPSLIMIT_MODERATE,
            PasswordHash.MEMLIMIT_MODERATE,
            PasswordHash.ALG_ARGON2ID13);
    }

    public byte[] GenerateSalt()
    {
        return RandomBytes.RandomBytes_Buf(PasswordHash.SALT_BYTES);
    }

    public byte[] Encrypt(byte[] textBytes, byte[] encryptionKeyBytes)
    {
        var nonce = RandomBytes.RandomBytes_Buf(SecretBox.NONCE_BYTES);
        var ciphertext = SecretBox.Crypto_SecretBox_Easy(textBytes, encryptionKeyBytes, nonce);
        
        var output = new byte[nonce.Length + ciphertext.Length];
        Buffer.BlockCopy(nonce, 0, output, 0, nonce.Length);
        Buffer.BlockCopy(ciphertext, 0, output, nonce.Length, ciphertext.Length);

        return output;
    }

    public byte[] Decrypt(byte[] encryptedTextBytes, byte[] encryptionKeyBytes)
    {
        var nonce = new byte[SecretBox.NONCE_BYTES];
        var ciphertext = new byte[encryptedTextBytes.Length - SecretBox.NONCE_BYTES];
        
        Buffer.BlockCopy(encryptedTextBytes, 0, nonce, 0, nonce.Length);
        Buffer.BlockCopy(encryptedTextBytes, nonce.Length, ciphertext, 0, ciphertext.Length);
        
        return SecretBox.Crypto_SecretBox_Open_Easy(ciphertext, encryptionKeyBytes, nonce);
    }
}