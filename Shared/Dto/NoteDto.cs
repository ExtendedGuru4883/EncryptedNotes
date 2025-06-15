using System.ComponentModel.DataAnnotations;
using Shared.Validations;

namespace Shared.Dto;

public class NoteDto
{
    public Guid Id { get; init; }
    [Required]
    [ValidBase64]
    public required string EncryptedTitleBase64 { get; init; }
    [Required]
    [ValidBase64]
    public required string EncryptedContentBase64 { get; init; }
    [Required]
    public DateTime TimeStamp { get; init; } = DateTime.UtcNow;
}