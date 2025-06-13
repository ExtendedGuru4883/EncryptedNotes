namespace Client.Services.Clients.Crypto.Interfaces;

public interface ICryptoService
{
    byte[] GenerateSalt (uint length);
}