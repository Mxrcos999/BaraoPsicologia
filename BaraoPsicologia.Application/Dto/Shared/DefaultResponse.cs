namespace BaraoPsicologia.Application.Dto.Shared;


public sealed class DefaultResponse
{
    public bool Sucess => Errors.Message.Count == 0;
    public object Data { get; set; }
    public Errors Errors { get; set; } = new Errors();

}

public class BaseResponse<T>
{
    public Errors Errors { get; set; } = new Errors();
    public bool Sucess => Errors.Message.Count == 0;
    public int PageSize { get; set; }
    public int Page { get; set; }
    public int TotalRecords { get; set; }
    public T Data { get; set; }
}