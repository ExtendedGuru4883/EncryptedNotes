using System.ComponentModel.DataAnnotations;

namespace Shared.Dto.Requests.Notes;

public class CursorPaginationNotesRequest
{
    [Required]
    public required DateTime DateTimeCursor { get; init; }
    [Required]
    [Range(1, 100)]
    public required int PageSize { get; init; }
}
