namespace Shared.Dto.Responses;

public class ErrorResponseDto
{
    public string ErrorMessage { get; set; } = string.Empty;
    
    public ErrorResponseDto() { }
    
    public ErrorResponseDto(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }
}