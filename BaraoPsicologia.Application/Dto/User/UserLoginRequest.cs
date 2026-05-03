using System.Text.Json.Serialization;

namespace BaraoPsicologia.Application.Dto.User;

public class UserLoginRequest
{
    [JsonPropertyName("username")]
    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
