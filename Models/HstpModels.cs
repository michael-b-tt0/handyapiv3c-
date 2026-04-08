using System.Text.Json.Serialization;

namespace handyapiv3.Models;

public sealed class DeviceTimeResponse
{
    [JsonPropertyName("time")]
    public long Time { get; set; }

    [JsonPropertyName("clock_offset")]
    public long ClockOffset { get; set; }

    [JsonPropertyName("rtd")]
    public int RoundTripDelay { get; set; }
}

public sealed class OffsetResponse
{
    [JsonPropertyName("offset")]
    public int Offset { get; set; }
}

public sealed class HstpOffsetRequest
{
    [JsonPropertyName("offset")]
    public int Offset { get; set; }
}
