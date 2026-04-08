using System.Text.Json.Serialization;

namespace handyapiv3.Models;

public sealed class HampStateResponse
{
    [JsonPropertyName("play_state")]
    public int PlayState { get; set; }

    [JsonPropertyName("velocity")]
    public double Velocity { get; set; }

    [JsonPropertyName("direction")]
    public bool Direction { get; set; }
}

public sealed class HampVelocityRequest
{
    [JsonPropertyName("velocity")]
    public double Velocity { get; set; }
}
