using System.Text.Json.Serialization;
using System.Text.Json;


namespace handyapiv3.Models;

public abstract class HsspSetupRequest
{
}

public sealed class HsspSetupUrlRequest : HsspSetupRequest
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}

public sealed class HsspPlayRequest
{
    [JsonPropertyName("start_time")]
    public long StartTime { get; set; }

    [JsonPropertyName("server_time")]
    public long ServerTime { get; set; }

    [JsonPropertyName("playback_rate")]
    public double PlaybackRate { get; set; }

    [JsonPropertyName("loop")]
    public bool Loop { get; set; }
}

public sealed class HsspResumeRequest
{
    [JsonPropertyName("pickUp")]
    public bool PickUp { get; set; }
}

public sealed class HsspSyncTimeRequest
{
    [JsonPropertyName("current_time")]
    public int CurrentTime { get; set; }

    [JsonPropertyName("server_time")]
    public long ServerTime { get; set; }

    [JsonPropertyName("filter")]
    public double Filter { get; set; }
}

public sealed class HsspStateResponse
{
    [JsonPropertyName("play_state")]
    public int PlayState { get; set; }

    [JsonPropertyName("pause_on_starving")]
    public bool PauseOnStarving { get; set; }

    [JsonPropertyName("points")]
    public int Points { get; set; }

    [JsonPropertyName("max_points")]
    public int MaxPoints { get; set; }

    [JsonPropertyName("current_point")]
    public int CurrentPoint { get; set; }

    [JsonPropertyName("current_time")]
    public int CurrentTime { get; set; }

    [JsonPropertyName("loop")]
    public bool Loop { get; set; }

    [JsonPropertyName("playback_rate")]
    public double PlaybackRate { get; set; }

    [JsonPropertyName("first_point_time")]
    public int FirstPointTime { get; set; }

    [JsonPropertyName("last_point_time")]
    public int LastPointTime { get; set; }

    [JsonPropertyName("stream_id")]
    public long StreamId { get; set; }

    [JsonPropertyName("tail_point_stream_index")]
    public int TailPointStreamIndex { get; set; }

    [JsonPropertyName("tail_point_stream_index_threshold")]
    public int TailPointStreamIndexThreshold { get; set; }
}
