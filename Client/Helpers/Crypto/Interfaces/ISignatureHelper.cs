using BlazorSodium.Sodium.Models;

namespace Client.Helpers.Crypto.Interfaces;

public interface ISignatureHelper
{
    Ed25519KeyPair GenerateKeyPair(byte[] passwordBytes, byte[] saltBytes);
    byte[] SignDetached(byte[] messageBytes, byte[] privateKeyBytes);
}