namespace Shared.Dto.Responses;

public class LoginResponse
{
    public required string Token { get; set; }
    public required string EncryptionSaltBase64 { get; set; }
}