using System.ComponentModel.DataAnnotations;
using Core.Entities.Base;

namespace Core.Entities;

public class UserEntity : BaseEntity
{
    [Required]
    public required string Username { get; set; }
    [Required]
    public required string SignatureSalt { get; set; }
    [Required]
    public required string EncryptionSalt { get; set; }
    [Required]
    public required string PublicKey { get; set; }

    //Navigation properties
    public IEnumerable<NoteEntity> Notes { get; set; } = [];
}