using System.ComponentModel.DataAnnotations;

namespace Shared.Dto.Requests;

public class AddNoteRequest
{
    [Required]
    public required string EncryptedTitleBase64 { get; init; }
    [Required]
    public required string EncryptedContentBase64 { get; init; }
}