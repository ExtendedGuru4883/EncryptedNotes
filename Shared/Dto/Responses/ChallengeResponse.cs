namespace Shared.Dto.Responses;

public class ChallengeResponse
{
    public required string SignatureSaltBase64 { get; set; }
    public required string NonceBase64 { get; set; }
}