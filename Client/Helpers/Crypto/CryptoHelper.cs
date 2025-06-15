using System.Runtime.Versioning;
using BlazorSodium.Sodium;
using Client.Helpers.Crypto.Interfaces;

namespace Client.Helpers.Crypto;

[SupportedOSPlatform("browser")]
public class CryptoHelper : ICryptoHelper
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
        var cipherText = SecretBox.Crypto_SecretBox_Easy(textBytes, encryptionKeyBytes, nonce);
        
        var output = new byte[nonce.Length + cipherText.Length];
        Buffer.BlockCopy(nonce, 0, output, 0, nonce.Length);
        Buffer.BlockCopy(cipherText, 0, output, nonce.Length, cipherText.Length);

        return output;
    }
}