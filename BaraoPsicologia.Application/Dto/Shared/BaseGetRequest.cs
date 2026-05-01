namespace BaraoPsicologia.Application.Dto.Shared;

public class BaseGetRequest
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? SearchInput { get; set; }
}
