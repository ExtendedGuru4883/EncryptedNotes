namespace Client.Services.Crypto.Interfaces;

public interface ICryptoService
{
    byte[] GenerateSalt (uint length);
}