namespace Shared.Dto.Responses;

public class ChallengeResponse
{
    public required string SignatureSaltBase64 { get; init; }
    public required string NonceBase64 { get; init; }
}