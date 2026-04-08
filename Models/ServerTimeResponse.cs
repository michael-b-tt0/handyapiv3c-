using System.Text.Json.Serialization;

namespace handyapiv3.Models;

public sealed class ServerTimeResponse
{
    [JsonPropertyName("server_time")]
    public long ServerTime { get; set; }
}
