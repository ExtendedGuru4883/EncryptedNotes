namespace Shared.Dto.Responses.Notes;

public record PageNumberPaginationResponse<T>
{
    public required List<T> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public bool HasMore => PageSize * Page < TotalCount;
}