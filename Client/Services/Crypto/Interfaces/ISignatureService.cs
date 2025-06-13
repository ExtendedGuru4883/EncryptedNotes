using BlazorSodium.Sodium.Models;

namespace Client.Services.Crypto.Interfaces;

public interface ISignatureService
{
    Ed25519KeyPair GenerateKeyPair(byte[] passwordBytes, byte[] saltBytes, uint length);
    byte[] SignDetached(byte[] messageBytes, byte[] privateKeyBytes);
}