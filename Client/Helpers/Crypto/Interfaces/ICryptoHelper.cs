namespace Client.Helpers.Crypto.Interfaces;

public interface ICryptoHelper
{
    byte[] GenerateSalt (uint length);
}