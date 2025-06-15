using System.ComponentModel.DataAnnotations;
using Shared.Validations;

namespace Shared.Dto.Requests;

public class AddNoteRequest
{
    [Required]
    [ValidBase64]
    [StringLength(256, ErrorMessage = "Encrypted title in base 64 cannot exceed 256 characters")]
    public required string EncryptedTitleBase64 { get; init; }
    [Required]
    [ValidBase64]
    [StringLength(2048, ErrorMessage = "Encrypted content in base 64 cannot exceed 2048 characters")]
    public required string EncryptedContentBase64 { get; init; }
}