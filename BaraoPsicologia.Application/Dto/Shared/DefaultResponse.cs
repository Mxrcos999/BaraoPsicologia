using System.Text.Json.Serialization;

namespace BaraoPsicologia.Application.Dto.Shared;

public sealed class DefaultResponse
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

    [JsonIgnore]
    public bool Sucess => Success;

    public object? Data { get; set; }
}

public class BaseResponse<T>
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

    [JsonIgnore]
    public bool Sucess => Success;

    public int PageSize { get; set; }
    public int Page { get; set; }
    public int TotalRecords { get; set; }
    public T? Data { get; set; }
}
