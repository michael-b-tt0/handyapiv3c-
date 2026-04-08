using System.Text.Json.Serialization;

namespace handyapiv3.Models;

public sealed class SliderSettings
{
    [JsonPropertyName("min")]
    public double Min { get; set; }

    [JsonPropertyName("max")]
    public double Max { get; set; }
}

public sealed class SliderStateResponse
{
    [JsonPropertyName("value")]
    public double Value { get; set; }
}

public sealed class SliderStrokeResponse
{
    [JsonPropertyName("min")]
    public double Min { get; set; }

    [JsonPropertyName("max")]
    public double Max { get; set; }

    [JsonPropertyName("min_absolute")]
    public double MinAbsolute { get; set; }

    [JsonPropertyName("max_absolute")]
    public double MaxAbsolute { get; set; }
}
