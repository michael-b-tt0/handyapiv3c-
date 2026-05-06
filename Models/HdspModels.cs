using System.Text.Json.Serialization;

namespace handyapiv3.Models;

public sealed class HdspStateResponse
{
    [JsonPropertyName("state")]
    public int State { get; set; }
}

public abstract class HdspCommandRequest
{
    [JsonPropertyName("stop_on_target")]
    public bool StopOnTarget { get; set; }

    [JsonPropertyName("immediate_rsp")]
    public bool ImmediateResponse { get; set; }
}

public sealed class HdspXavaRequest : HdspCommandRequest
{
    [JsonPropertyName("xa")]
    public double AbsolutePosition { get; set; }

    [JsonPropertyName("va")]
    public double AbsoluteVelocity { get; set; }
}

public sealed class HdspXavpRequest : HdspCommandRequest
{
    [JsonPropertyName("xa")]
    public double AbsolutePosition { get; set; }

    [JsonPropertyName("vp")]
    public double PercentVelocity { get; set; }
}

public sealed class HdspXpvaRequest : HdspCommandRequest
{
    [JsonPropertyName("xp")]
    public double NormalizedPosition { get; set; }

    [JsonPropertyName("va")]
    public double AbsoluteVelocity { get; set; }
}

public sealed class HdspXpvpRequest : HdspCommandRequest
{
    [JsonPropertyName("xp")]
    public double NormalizedPosition { get; set; }

    [JsonPropertyName("vp")]
    public double PercentVelocity { get; set; }
}

public sealed class HdspXatRequest : HdspCommandRequest
{
    [JsonPropertyName("xa")]
    public double AbsolutePosition { get; set; }

    [JsonPropertyName("t")]
    public double Duration { get; set; }
}

public sealed class HdspXptRequest : HdspCommandRequest
{
    [JsonPropertyName("xp")]
    public double NormalizedPosition { get; set; }

    [JsonPropertyName("t")]
    public double Duration { get; set; }
}
