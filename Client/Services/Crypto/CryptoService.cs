using System.Runtime.Versioning;
using BlazorSodium.Sodium;
using Client.Services.Crypto.Interfaces;

namespace Client.Services.Crypto;

[SupportedOSPlatform("browser")]
public class CryptoService : ICryptoService
{
    public byte[] GenerateSalt(uint length)
    {
        return RandomBytes.RandomBytes_Buf(length);
    }
}