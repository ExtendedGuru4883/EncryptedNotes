namespace Core.Abstractions.BusinessLogic.Services;

public interface ICryptoService
{
    byte[] GetRandomBytes(int count);
}