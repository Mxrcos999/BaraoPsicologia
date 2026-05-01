namespace BaraoPsicologia.Application.Dto.Shared;

public class Errors
{
    public List<object> Message { get; set; } = new List<object>();
    public void AddError(string message)
    {
        Message.Add(message);
    }
}
