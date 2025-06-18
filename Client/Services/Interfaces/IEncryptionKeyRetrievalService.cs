namespace Client.Services.Interfaces;

public interface IEncryptionKeyRetrievalService
{
    Task<byte[]?> TryGetKeyAsync();
    Task<byte[]> GetKeyOrThrowAsync();
}