using System.Runtime.Versioning;
using BlazorSodium.Sodium;
using Client.Helpers.Crypto.Interfaces;

namespace Client.Helpers.Crypto;

[SupportedOSPlatform("browser")]
public class CryptoHelper : ICryptoHelper
{
    public byte[] GenerateSalt(uint length)
    {
        return RandomBytes.RandomBytes_Buf(length);
    }
}