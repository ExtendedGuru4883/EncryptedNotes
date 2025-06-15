using System.ComponentModel.DataAnnotations;
using Core.Entities.Base;

namespace Core.Entities;

public class NoteEntity : BaseEntity
{
    [Required]
    public required string EncryptedTitleBase64 { get; init; }
    [Required]
    public required string EncryptedContentBase64 { get; init; }
    [Required]
    public DateTime TimeStamp { get; init; } = DateTime.UtcNow;
    
    //Foreign keys
    [Required]
    public required Guid UserId { get; set; }
    
    //Navigation properties
    public virtual UserEntity? User { get; init; }
}