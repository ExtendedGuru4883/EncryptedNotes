using BlazorSodium.Sodium.Models;

namespace Client.Services.Interfaces;

public interface ISignatureService
{
    Ed25519KeyPair GenerateKeyPair(byte[] passwordBytes, byte[] saltBytes);
    byte[] SignDetached(byte[] messageBytes, byte[] privateKeyBytes);
}