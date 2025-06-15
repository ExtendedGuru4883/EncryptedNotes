using System.Runtime.Versioning;
using BlazorSodium.Sodium;
using BlazorSodium.Sodium.Models;
using Client.Helpers.Crypto.Interfaces;

namespace Client.Helpers.Crypto;

[SupportedOSPlatform("browser")]
public class SignatureHelper : ISignatureHelper
{
    public Ed25519KeyPair GenerateKeyPair(byte[] passwordBytes, byte[] saltBytes)
    {
        var seed = PasswordHash.Crypto_PwHash(
            PublicKeySignature.SEED_BYTES,
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