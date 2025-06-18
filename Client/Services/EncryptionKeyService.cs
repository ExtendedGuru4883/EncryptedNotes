using Blazored.SessionStorage;
using Client.Services.Interfaces;

namespace Client.Services;

public class EncryptionKeyService(ISessionStorageService sessionStorageService) : IEncryptionKeyService
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
}