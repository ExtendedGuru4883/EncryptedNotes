namespace BusinessLogic.Helpers.Crypto.Interfaces;

public interface ICryptoHelper
{
    byte[] GetRandomBytes(int count);
}