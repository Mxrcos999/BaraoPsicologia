using BaraoPsicologia.Application.Dto.Shared;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BaraoPsicologia.Application.Dto.User;

public sealed class UserRegisterResponse
{
    public bool Success => Errors.Message.Count == 0 ? true : false;

    public Errors Errors { get; set; } = new Errors();
    public string Data { get; set; }


    public UserRegisterResponse(bool success)
    {
    }
}
