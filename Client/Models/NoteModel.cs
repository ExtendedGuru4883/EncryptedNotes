using System.ComponentModel.DataAnnotations;
using Client.Models.Base;

namespace Client.Models;

public class NoteModel : BaseNoteModel
{
    public required Guid Id { get; init; }
    public required DateTime TimeStamp { get; set; }

}