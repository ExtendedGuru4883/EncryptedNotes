using Core.Abstractions.BusinessLogic.Services;
using Sodium;

namespace BusinessLogic.Services;

public class CryptoService : ICryptoService
{
    public byte[] GetRandomBytes(int count)
    {
        return SodiumCore.GetRandomBytes(count);
    }
}