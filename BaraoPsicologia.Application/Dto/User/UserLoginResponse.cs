using BaraoPsicologia.Application.Dto.Shared;
using System.Text.Json.Serialization;

namespace BaraoPsicologia.Application.Dto.User;

public class UserLoginResponse
{
    [JsonIgnore]
    public Errors Errors { get; set; } = new();

    [JsonPropertyName("errors")]
    public Dictionary<string, string[]>? ErrorsPayload =>
        Errors.Message.Count == 0
            ? null
            : new Dictionary<string, string[]>
            {
                ["_error"] = Errors.Message.Select(e => e?.ToString() ?? "").ToArray()
            };

    [JsonPropertyName("success")]
    public bool Success => Errors.Message.Count == 0;

    public string Email { get; private set; } = string.Empty;
    public string Type { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Id { get; private set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? AccessToken { get; private set; }

    /// <summary>Segundos até expiração (número esperado pela UI).</summary>
    public int ExpirationTimeAccessToken { get; private set; }

    public DateTime ExpirationDateTimeAccessToken { get; private set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? RefreshToken { get; private set; }

    public int ExpirationTimeRefreshtoken { get; private set; }
    public DateTime ExpirationDateTimeRefreshtoken { get; private set; }

    public UserLoginResponse(bool success)
    {
    }

    public UserLoginResponse(bool success, string type, string accessToken, string refreshToken, int expirationTimeRefreshtoken, int expirationTimeAccessToken, string name, string id, string email)
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
