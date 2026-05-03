using BaraoPsicologia.Application.Dto.Shared;
using System.Text.Json.Serialization;

namespace BaraoPsicologia.Application.Dto.User;

public sealed class UserRegisterResponse
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

    public string? Data { get; set; }

    public UserRegisterResponse(bool success)
    {
    }
}
