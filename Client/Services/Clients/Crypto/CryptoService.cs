using System.Runtime.Versioning;
using Client.Services.Clients.Crypto.Interfaces;
using BlazorSodium.Sodium;

namespace Client.Services.Clients.Crypto;

[SupportedOSPlatform("browser")]
public class CryptoService : ICryptoService
{
    public byte[] GenerateSalt(uint length)
    {
        return RandomBytes.RandomBytes_Buf(length);
    }
}