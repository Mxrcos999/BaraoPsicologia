using BaraoPsicologia.Application.Dto.Shared;
using System.Text.Json.Serialization;

namespace BaraoPsicologia.Application.Dto.User;

public class UserLoginResponse
{

    public bool Success => Errors.Message.Count == 0 ? true : false;

    public string Email { get; private set; }
    public string Type { get; private set; }
    public string Name { get; private set; }
    public string Id { get; private set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string AccessToken { get; private set; }
    public string? ExpirationTimeAccessToken { get; private set; }
    public DateTime ExpirationDateTimeAccessToken { get; private set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string RefreshToken { get; private set; }
    public string? ExpirationTimeRefreshtoken { get; private set; }
    public DateTime ExpirationDateTimeRefreshtoken { get; private set; }
    public Errors Errors { get; set; } = new Errors();


    public UserLoginResponse(bool success)
    { }

    public UserLoginResponse(bool success, string type, string accessToken, string refreshToken, string expirationTimeRefreshtoken, string expirationTimeAccessToken, string name, string id, string email)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        Type = type;
        ExpirationTimeAccessToken = expirationTimeAccessToken;
        ExpirationTimeRefreshtoken = expirationTimeRefreshtoken;
        ExpirationDateTimeAccessToken = DateTime.Now.AddSeconds(3000);
        ExpirationDateTimeRefreshtoken = DateTime.Now.AddSeconds(10200);
        Name = name;
        Id = id;
        Email = email;
    }

    public UserLoginResponse(bool success, string accessToken, string refreshToken, string email)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        Email = email;
    }
}
