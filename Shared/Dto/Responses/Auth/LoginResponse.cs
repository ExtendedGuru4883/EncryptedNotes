namespace Shared.Dto.Responses.Auth;

public record LoginResponse
{
    public required string Token { get; init; }
    public required string EncryptionSaltBase64 { get; init; }
}