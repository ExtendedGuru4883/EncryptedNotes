using System.ComponentModel.DataAnnotations;

namespace Client.Models;

public class AddNoteFormModel
{
    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; } = string.Empty;
    [Required(ErrorMessage = "Content is required")]
    public string Content { get; set; } = string.Empty;
}