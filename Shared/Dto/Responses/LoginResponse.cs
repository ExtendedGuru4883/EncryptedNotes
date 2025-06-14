namespace Shared.Dto.Responses;

public class LoginResponse
{
    public required string Token { get; init; }
    public required string EncryptionSaltBase64 { get; init; }
}