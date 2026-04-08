using System.Text.Json.Serialization;

namespace handyapiv3.Models;

public sealed class HandyApiError
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("connected")]
    public bool? Connected { get; set; }
}
