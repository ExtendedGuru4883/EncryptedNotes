using System.ComponentModel.DataAnnotations;

namespace Shared.Dto.Requests;

public class PaginatedNotesRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public required int Page { get; init; }
    [Required]
    [Range(1, 100)]
    public required int PageSize { get; init; }
}