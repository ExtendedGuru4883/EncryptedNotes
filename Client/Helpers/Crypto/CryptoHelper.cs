using System.Runtime.Versioning;
using BlazorSodium.Sodium;
using Client.Helpers.Crypto.Interfaces;

namespace Client.Helpers.Crypto;

[SupportedOSPlatform("browser")]
public class CryptoHelper : ICryptoHelper
{
    public byte[] DeriveEncryptionKey(byte[] passwordBytes, byte[] saltBytes, uint keyLength)
    {
        return PasswordHash.Crypto_PwHash(
            keyLength,
            passwordBytes,
            saltBytes,
            PasswordHash.OPSLIMIT_MODERATE,
            PasswordHash.MEMLIMIT_MODERATE,
            PasswordHash.ALG_ARGON2ID13);
    }

    public byte[] GenerateSalt(uint length)
    {
        return RandomBytes.RandomBytes_Buf(length);
    }
}