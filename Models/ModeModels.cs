using System.Text.Json.Serialization;

namespace handyapiv3.Models;

public sealed class GetModeResult
{
    [JsonPropertyName("mode")]
    public int Mode { get; set; }

    [JsonPropertyName("mode_session_id")]
    public int ModeSessionId { get; set; }
}

public sealed class PutModeRequest
{
    [JsonPropertyName("mode")]
    public int Mode { get; set; }
}

public sealed class ModeResponse
{
    [JsonPropertyName("mode")]
    public int Mode { get; set; }

    [JsonPropertyName("mode_session_id")]
    public int ModeSessionId { get; set; }
}

public sealed class ConnectedResponse
{
    [JsonPropertyName("connected")]
    public bool Connected { get; set; }
}
