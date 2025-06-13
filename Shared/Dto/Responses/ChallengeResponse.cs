namespace Shared.Dto.Responses;

public class ChallengeResponse
{
    public required string SignatureSalt { get; set; }
    public required string Nonce { get; set; }
}