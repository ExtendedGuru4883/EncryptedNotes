namespace Client.Services.Interfaces;

public interface IEncryptionKeyStorageService
{
    Task<byte[]?> TryGetKeyAsync();
    Task<byte[]> GetKeyOrThrowAsync();
}