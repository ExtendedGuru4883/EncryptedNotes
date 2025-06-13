using System.Runtime.Versioning;
using BlazorSodium.Sodium;
using BlazorSodium.Sodium.Models;
using Client.Services.Clients.Crypto.Interfaces;

namespace Client.Services.Clients.Crypto;

[SupportedOSPlatform("browser")]
public class SignatureService : ISignatureService
{
    public Ed25519KeyPair GenerateKeyPair(byte[] passwordBytes, byte[] saltBytes, uint keyLength)
    {
        var seed = PasswordHash.Crypto_PwHash(
            keyLength,
            passwordBytes,
            saltBytes,
            PasswordHash.OPSLIMIT_MODERATE,
            PasswordHash.MEMLIMIT_MODERATE,
            PasswordHash.ALG_ARGON2ID13);

        return PublicKeySignature.Crypto_Sign_Seed_KeyPair(seed);
    }

    public byte[] SignDetached(byte[] messageBytes, byte[] privateKeyBytes)
    {
        return PublicKeySignature.Crypto_Sign_Detached(messageBytes, privateKeyBytes);
    }
}