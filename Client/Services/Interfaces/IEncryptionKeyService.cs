namespace Client.Services.Interfaces;

public interface IEncryptionKeyService
{
    Task<byte[]?> TryGetKeyAsync();
}