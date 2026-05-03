namespace BaraoPsicologia.Application.Dto.Shared;

public sealed class PagedResult<T>
{
    public List<T> Data { get; set; } = new();
    public int TotalRecords { get; set; }
}
