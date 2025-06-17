namespace Shared.Dto.Responses;

public record ErrorResponseDto
{
    public string ErrorMessage { get; init; } = string.Empty;
    
    public ErrorResponseDto() { }
    
    public ErrorResponseDto(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }
}