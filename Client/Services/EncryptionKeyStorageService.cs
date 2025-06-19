using Blazored.SessionStorage;
using Client.Exceptions;
using Client.Services.Interfaces;

namespace Client.Services;

public class EncryptionKeyStorageService(ISessionStorageService sessionStorageService) : IEncryptionKeyStorageService
{
    public async Task<byte[]?> TryGetKeyAsync()
    {
        var encryptionKeyBase64 = await sessionStorageService.GetItemAsStringAsync("encryptionKeyBase64");
        if (string.IsNullOrEmpty(encryptionKeyBase64)) return null;
        try
        {
            return Convert.FromBase64String(encryptionKeyBase64);
        }
        catch
        {
            return null;
        }
    }

    public async Task<byte[]> GetKeyOrThrowAsync()
    {
        return (await TryGetKeyAsync()) ??
               throw new EncryptionKeyMissingException("Encryption key is missing or not in correct format");
    }
}