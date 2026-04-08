using System.Text.Json.Serialization;

namespace handyapiv3.Models;

public class HandyApiResponse
{
    [JsonPropertyName("error")]
    public HandyApiError? Error { get; set; }

    public int RateLimitPerMinute { get; set; }

    public int RateLimitRemaining { get; set; }

    public int MsUntilRateLimitReset { get; set; }
}

public sealed class HandyApiResponse<T> : HandyApiResponse
{
    [JsonPropertyName("result")]
    public T? Result { get; set; }
}
