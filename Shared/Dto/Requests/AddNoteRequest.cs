using System.ComponentModel.DataAnnotations;
using Shared.Validations;

namespace Shared.Dto.Requests;

public class AddNoteRequest
{
    [Required]
    [ValidBase64]
    public required string EncryptedTitleBase64 { get; init; }
    [Required]
    [ValidBase64]
    public required string EncryptedContentBase64 { get; init; }
}