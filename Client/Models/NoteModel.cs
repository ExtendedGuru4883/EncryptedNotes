namespace Client.Models;

public class NoteModel
{
    public required string Title { get; init; } = string.Empty;
    public required string Content { get; init; } = string.Empty;
    public required DateTime TimeStamp { get; init; }

}