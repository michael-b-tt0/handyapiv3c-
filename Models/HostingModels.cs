using System.Text.Json.Serialization;

namespace handyapiv3.Models;

public sealed class ScriptUploadResponse
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}

public sealed class HostingApiErrorResponse
{
    [JsonPropertyName("error")]
    public HandyApiError? Error { get; set; }
}
