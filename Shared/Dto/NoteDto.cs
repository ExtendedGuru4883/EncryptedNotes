using System.ComponentModel.DataAnnotations;

namespace Shared.Dto;

public class NoteDto
{
    public Guid Id { get; init; }
    [Required]
    public required string EncryptedTitleBase64 { get; init; }
    [Required]
    public required string EncryptedContentBase64 { get; init; }
    [Required]
    public DateTime TimeStamp { get; init; } = DateTime.UtcNow;
}