namespace Shared.Dto.Responses;

public record LoginResponse
{
    public required string Token { get; init; }
    public required string EncryptionSaltBase64 { get; init; }
}