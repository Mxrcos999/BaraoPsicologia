using System.Text.Json.Serialization;

namespace BaraoPsicologia.Application.Dto.User;

public class PatchUserRequest
{
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
}

/// <summary>Corpo de PUT /user/update-user (IUser completo no formulário).</summary>
public class UpdateUserRequest
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Profile { get; set; } = string.Empty;
    public bool ReceiveEmails { get; set; }
}
