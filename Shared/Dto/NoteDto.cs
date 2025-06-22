using System.ComponentModel.DataAnnotations;
using Shared.Constants;
using Shared.Validations;

namespace Shared.Dto;

public record NoteDto
{
    public Guid Id { get; init; }
    [Required]
    [ValidBase64]
    [CustomStringLength(NoteConstants.EncryptedTitleBase64MaxLength, 1)]
    public required string EncryptedTitleBase64 { get; init; }
    [Required]
    [ValidBase64]
    [CustomStringLength(NoteConstants.EncryptedContentBase64MaxLength, 1)]
    public required string EncryptedContentBase64 { get; init; }
    [Required]
    public DateTime TimeStamp { get; init; } = DateTime.UtcNow;
}