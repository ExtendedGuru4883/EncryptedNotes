using System.ComponentModel.DataAnnotations;
using Core.Entities.Base;

namespace Core.Entities;

public class NoteEntity : BaseEntity
{
    [Required]
    public required string Title { get; set; }
    [Required]
    public required string Content { get; set; }
    [Required]
    public DateTime TimeStamp { get; set; } = DateTime.Now;
    
    //Foreign keys
    [Required]
    public required Guid UserId { get; set; }
    
    //Navigation properties
    public virtual UserEntity? User { get; set; }
}