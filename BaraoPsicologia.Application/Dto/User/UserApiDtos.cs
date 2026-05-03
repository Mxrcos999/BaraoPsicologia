using System.Text.Json.Serialization;

namespace BaraoPsicologia.Application.Dto.User;

public sealed class ForgotPasswordRequest
{
    public string Email { get; set; } = string.Empty;
}

public sealed class PostUserRequest
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Profile { get; set; } = string.Empty;
}

public sealed class SetReceiveEmailRequest
{
    [JsonPropertyName("receiveEmail")]
    public bool ReceiveEmail { get; set; }
}

public sealed class UserOptionResponse
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public sealed class UserListItemResponse
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Profile { get; set; } = string.Empty;
    public bool ReceiveEmails { get; set; }
}
