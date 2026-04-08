using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using handyapiv3.Models;

namespace handyapiv3;

public sealed class HandyApiV3Client
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly HttpClient _httpClient;
    private readonly HandyApiV3ClientOptions _options;

    public HandyApiV3Client(HttpClient httpClient, HandyApiV3ClientOptions? options = null)
    {
        _httpClient = httpClient;
        _options = options ?? new HandyApiV3ClientOptions();
    }

    public string? ConnectionKey
    {
        get => _options.ConnectionKey;
        set => _options.ConnectionKey = value;
    }

    public Task<HandyApiResponse<GetModeResult>> GetModeAsync(CancellationToken cancellationToken = default)
        => GetAsync<GetModeResult>("mode", cancellationToken);

    public Task<HandyApiResponse<ModeResponse>> PutModeAsync(HandyMode mode, CancellationToken cancellationToken = default)
        => PutAsync<ModeResponse>("mode2", new PutModeRequest { Mode = (int)mode }, cancellationToken);

    public Task<HandyApiResponse<ConnectedResponse>> GetConnectedAsync(CancellationToken cancellationToken = default)
        => GetAsync<ConnectedResponse>("connected", cancellationToken);

    public Task<HandyApiResponse<InfoResponse>> GetInfoAsync(CancellationToken cancellationToken = default)
        => GetAsync<InfoResponse>("info", cancellationToken);

    public Task<HandyApiResponse<HsspStateResponse>> GetHsspStateAsync(CancellationToken cancellationToken = default)
        => GetAsync<HsspStateResponse>("hssp/state", cancellationToken);

    public Task<HandyApiResponse<HsspStateResponse>> SetupHsspAsync(HsspSetupRequest request, CancellationToken cancellationToken = default)
        => PutAsync<HsspStateResponse>("hssp/setup", request, cancellationToken);

    public Task<HandyApiResponse<HsspStateResponse>> PlayHsspAsync(HsspPlayRequest request, CancellationToken cancellationToken = default)
        => PutAsync<HsspStateResponse>("hssp/play", request, cancellationToken);

    public Task<HandyApiResponse<HsspStateResponse>> StopHsspAsync(CancellationToken cancellationToken = default)
        => PutAsync<HsspStateResponse>("hssp/stop", null, cancellationToken);

    public Task<HandyApiResponse<HsspStateResponse>> SyncHsspTimeAsync(HsspSyncTimeRequest request, CancellationToken cancellationToken = default)
        => PutAsync<HsspStateResponse>("hssp/synctime", request, cancellationToken);

    public Task<HandyApiResponse<DeviceTimeResponse>> GetHstpInfoAsync(CancellationToken cancellationToken = default)
        => GetAsync<DeviceTimeResponse>("hstp/info", cancellationToken);

    public Task<HandyApiResponse<OffsetResponse>> GetHstpOffsetAsync(CancellationToken cancellationToken = default)
        => GetAsync<OffsetResponse>("hstp/offset", cancellationToken);

    public Task<HandyApiResponse<string>> PutHstpOffsetAsync(HstpOffsetRequest request, CancellationToken cancellationToken = default)
        => PutAsync<string>("hstp/offset", request, cancellationToken);

    public Task<ServerTimeResponse> GetServerTimeAsync(CancellationToken cancellationToken = default)
        => GetRawAsync<ServerTimeResponse>("servertime", cancellationToken);

    public Task<HandyApiResponse<HampStateResponse>> GetHampStateAsync(CancellationToken cancellationToken = default)
        => GetAsync<HampStateResponse>("hamp/state", cancellationToken);

    public Task<HandyApiResponse<HampStateResponse>> StartHampAsync(CancellationToken cancellationToken = default)
        => PutAsync<HampStateResponse>("hamp/start", null, cancellationToken);

    public Task<HandyApiResponse<HampStateResponse>> StopHampAsync(CancellationToken cancellationToken = default)
        => PutAsync<HampStateResponse>("hamp/stop", null, cancellationToken);

    public Task<HandyApiResponse<HampStateResponse>> PutHampVelocityAsync(double velocity, CancellationToken cancellationToken = default)
        => PutAsync<HampStateResponse>("hamp/velocity", new HampVelocityRequest { Velocity = velocity }, cancellationToken);

    public Task<HandyApiResponse<SliderStateResponse>> GetSliderStateAsync(CancellationToken cancellationToken = default)
        => GetAsync<SliderStateResponse>("slider/state", cancellationToken);

    public Task<HandyApiResponse<SliderStrokeResponse>> GetSliderStrokeAsync(CancellationToken cancellationToken = default)
        => GetAsync<SliderStrokeResponse>("slider/stroke", cancellationToken);

    public Task<HandyApiResponse<SliderStrokeResponse>> PutSliderStrokeAsync(SliderSettings request, CancellationToken cancellationToken = default)
        => PutAsync<SliderStrokeResponse>("slider/stroke", request, cancellationToken);

    private Task<HandyApiResponse<T>> GetAsync<T>(string relativeUrl, CancellationToken cancellationToken) where T : class
    {
        var request = CreateRequest(HttpMethod.Get, relativeUrl);
        return SendEnvelopeAsync<T>(request, cancellationToken);
    }

    private Task<HandyApiResponse<T>> PutAsync<T>(string relativeUrl, object? data, CancellationToken cancellationToken) where T : class
    {
        var request = CreateRequest(HttpMethod.Put, relativeUrl, data);
        return SendEnvelopeAsync<T>(request, cancellationToken);
    }

    private async Task<T> GetRawAsync<T>(string relativeUrl, CancellationToken cancellationToken) where T : class
    {
        using var request = CreateRequest(HttpMethod.Get, relativeUrl);
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return (await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions, cancellationToken))
            ?? throw new InvalidOperationException("Handy API returned an empty payload.");
    }

    private async Task<HandyApiResponse<T>> SendEnvelopeAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken) where T : class
    {
        using (request)
        using (var response = await _httpClient.SendAsync(request, cancellationToken))
        {
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var payload = await JsonSerializer.DeserializeAsync<HandyApiResponse<T>>(stream, JsonOptions, cancellationToken)
                ?? throw new InvalidOperationException("Handy API returned an empty payload.");

            payload.RateLimitPerMinute = TryParseHeader(response.Headers, "X-RateLimit-Limit");
            payload.RateLimitRemaining = TryParseHeader(response.Headers, "X-RateLimit-Remaining");
            payload.MsUntilRateLimitReset = TryParseHeader(response.Headers, "X-RateLimit-Reset");

            return payload;
        }
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string relativeUrl, object? data = null)
    {
        if (string.IsNullOrWhiteSpace(ConnectionKey))
        {
            throw new InvalidOperationException("A Handy connection key must be set before calling the API.");
        }

        var request = new HttpRequestMessage(method, BuildUri(relativeUrl));
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.Add("X-Connection-Key", ConnectionKey);
        request.Headers.Add("X-Api-Key", _options.ApplicationApiKey);

        if (data is not null)
        {
            var json = JsonSerializer.Serialize(data, JsonOptions);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        return request;
    }

    private Uri BuildUri(string relativeUrl)
    {
        var baseUri = _options.ApiBaseUrl.EndsWith("/", StringComparison.Ordinal)
            ? _options.ApiBaseUrl
            : _options.ApiBaseUrl + "/";

        var path = relativeUrl.StartsWith("/", StringComparison.Ordinal)
            ? relativeUrl[1..]
            : relativeUrl;

        return new Uri(new Uri(baseUri, UriKind.Absolute), path);
    }

    private static int TryParseHeader(HttpResponseHeaders headers, string name)
    {
        if (!headers.TryGetValues(name, out var values))
        {
            return 0;
        }

        foreach (var value in values)
        {
            if (int.TryParse(value, out var parsed))
            {
                return parsed;
            }
        }

        return 0;
    }
}
