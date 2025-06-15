using System.ComponentModel.DataAnnotations;

namespace Shared.Dto;

public class NoteDto
{
    [Required]
    public required string EncryptedTitleBase64 { get; set; }
    [Required]
    public required string EncryptedContentBase64 { get; set; }
    [Required]
    public required DateTime TimeStamp { get; set; }
}