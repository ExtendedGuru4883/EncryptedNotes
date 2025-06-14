using BusinessLogic.Helpers.Crypto.Interfaces;
using Sodium;

namespace BusinessLogic.Helpers.Crypto;

public class CryptoHelper : ICryptoHelper
{
    public byte[] GetRandomBytes(int count)
    {
        return SodiumCore.GetRandomBytes(count);
    }
}