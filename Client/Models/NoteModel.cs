using Client.Models.Base;

namespace Client.Models;

public record NoteModel : BaseNoteModel
{
    public required Guid Id { get; init; }
    public required DateTime TimeStamp { get; set; }

}