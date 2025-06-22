using System.ComponentModel.DataAnnotations;
using Core.Entities.Base;

namespace Core.Entities;

public record UserEntity : BaseEntity
{
    [Required]
    public required string Username { get; set; }
    [Required]
    public required string SignatureSaltBase64 { get; set; }
    [Required]
    public required string EncryptionSaltBase64 { get; set; }
    [Required]
    public required string PublicKeyBase64 { get; set; }

    //Navigation properties
    public IEnumerable<NoteEntity> Notes { get; set; } = new List<NoteEntity>();
}