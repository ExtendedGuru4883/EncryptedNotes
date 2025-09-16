namespace Shared.Dto.Responses.Notes;

public class CursorPaginationResponse<T>
{
    public required List<T> Items { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    
    
}