using System.ComponentModel.DataAnnotations;
using Shared.Constants;
using Shared.Validations;

namespace Client.Models.Base;

public record BaseNoteModel
{
    [Required(ErrorMessage = "Title is required")]
    [CustomStringLength(NoteConstants.TitleMaxLength, 1)]
    public string Title { get; set; } = string.Empty;
    [Required(ErrorMessage = "Content is required")]
    [CustomStringLength(NoteConstants.ContentMaxLength, 1)]
    public string Content { get; set; } = string.Empty;
}