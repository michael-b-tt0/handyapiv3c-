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

    public Task<ScriptUploadResponse> UploadScriptAsync(string fileName, byte[] content, string? contentType = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        if (content.Length == 0)
        {
            throw new ArgumentException("Script content is required.", nameof(content));
        }

        var fileContent = new ByteArrayContent(content);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(ResolveContentType(fileName, contentType));

        var form = new MultipartFormDataContent();
        form.Add(fileContent, "file", fileName);

        var request = CreateHostingRequest(HttpMethod.Post, "upload", form);
        return SendHostingAsync<ScriptUploadResponse>(request, cancellationToken);
    }

    public Task<HandyApiResponse<HsspStateResponse>> PlayHsspAsync(HsspPlayRequest request, CancellationToken cancellationToken = default)
        => PutAsync<HsspStateResponse>("hssp/play", request, cancellationToken);

    public Task<HandyApiResponse<HsspStateResponse>> PauseHsspAsync(CancellationToken cancellationToken = default)
        => PutAsync<HsspStateResponse>("hssp/pause", null, cancellationToken);

    public Task<HandyApiResponse<HsspStateResponse>> ResumeHsspAsync(HsspResumeRequest request, CancellationToken cancellationToken = default)
        => PutAsync<HsspStateResponse>("hssp/resume", request, cancellationToken);

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

    public Task<HandyApiResponse<string>> PutHdspXavaAsync(HdspXavaRequest request, CancellationToken cancellationToken = default)
        => PutAsync<string>("hdsp/xava", request, cancellationToken);

    public Task<HandyApiResponse<string>> PutHdspXavpAsync(HdspXavpRequest request, CancellationToken cancellationToken = default)
        => PutAsync<string>("hdsp/xavp", request, cancellationToken);

    public Task<HandyApiResponse<string>> PutHdspXpvaAsync(HdspXpvaRequest request, CancellationToken cancellationToken = default)
        => PutAsync<string>("hdsp/xpva", request, cancellationToken);

    public Task<HandyApiResponse<string>> PutHdspXpvpAsync(HdspXpvpRequest request, CancellationToken cancellationToken = default)
        => PutAsync<string>("hdsp/xpvp", request, cancellationToken);

    public Task<HandyApiResponse<string>> PutHdspXatAsync(HdspXatRequest request, CancellationToken cancellationToken = default)
        => PutAsync<string>("hdsp/xat", request, cancellationToken);

    public Task<HandyApiResponse<string>> PutHdspXptAsync(HdspXptRequest request, CancellationToken cancellationToken = default)
        => PutAsync<string>("hdsp/xpt", request, cancellationToken);

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
            // Log the complete request before sending
           /* Console.WriteLine("========== FULL HTTP REQUEST ==========");
            Console.WriteLine($"Method: {request.Method}");
            Console.WriteLine($"URI: {request.RequestUri}");
            Console.WriteLine("Headers:");
            foreach (var header in request.Headers)
                {
                Console.WriteLine($"  {header.Key}: {string.Join(", ", header.Value)}");
                }*/
            if (request.Content != null)
                {
                /*Console.WriteLine("Content Headers:");
                foreach (var header in request.Content.Headers)
                    {
                    Console.WriteLine($"  {header.Key}: {string.Join(", ", header.Value)}");
                    }*/
                var requestBody = await request.Content.ReadAsStringAsync(cancellationToken);
                /*Console.WriteLine("Body:");
                Console.WriteLine(requestBody);*/
                }
           /* Console.WriteLine("========== END REQUEST ==========");*/

            // Log the raw response before EnsureSuccessStatusCode throws
            var rawBody = await response.Content.ReadAsStringAsync(cancellationToken);
            /*Console.WriteLine($"[HandyApiV3Client] {request.Method} {request.RequestUri} -> {(int)response.StatusCode}");
            Console.WriteLine($"[HandyApiV3Client] Response body: {rawBody}");*/

            response.EnsureSuccessStatusCode();

            // Parse from the string we already read
            var payload = JsonSerializer.Deserialize<HandyApiResponse<T>>(rawBody, JsonOptions)
                ?? throw new InvalidOperationException("Handy API returned an empty payload.");

            payload.RateLimitPerMinute = TryParseHeader(response.Headers, "X-RateLimit-Limit");
            payload.RateLimitRemaining = TryParseHeader(response.Headers, "X-RateLimit-Remaining");
            payload.MsUntilRateLimitReset = TryParseHeader(response.Headers, "X-RateLimit-Reset");

            return payload;
            }
        }

    private async Task<T> SendHostingAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken) where T : class
    {
        using (request)
        using (var response = await _httpClient.SendAsync(request, cancellationToken))
        {
            var rawBody = await response.Content.ReadAsStringAsync(cancellationToken);
            Console.WriteLine($"[HandyApiV3Client] {request.Method} {request.RequestUri} -> {(int)response.StatusCode}");
            Console.WriteLine($"[HandyApiV3Client] Response body: {rawBody}");

            if (!response.IsSuccessStatusCode)
            {
                var apiError = TryDeserialize<HandyApiError>(rawBody)
                    ?? TryDeserialize<HostingApiErrorResponse>(rawBody)?.Error;

                throw new InvalidOperationException(apiError?.Message ?? apiError?.Name ?? $"Hosting API returned {(int)response.StatusCode}.");
            }

            return JsonSerializer.Deserialize<T>(rawBody, JsonOptions)
                ?? throw new InvalidOperationException("Hosting API returned an empty payload.");
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
            Console.WriteLine($"[HandyApiV3Client] Request body ({data.GetType().Name}): {json}");
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        return request;
    }

    private HttpRequestMessage CreateHostingRequest(HttpMethod method, string relativeUrl, HttpContent? content = null)
    {
        var request = new HttpRequestMessage(method, BuildUri(_options.HostingApiBaseUrl, relativeUrl));
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        if (content is not null)
        {
            request.Content = content;
        }

        return request;
    }

    private Uri BuildUri(string baseUri, string relativeUrl)
    {
        var normalizedBaseUri = baseUri.EndsWith("/", StringComparison.Ordinal)
            ? baseUri
            : baseUri + "/";

        var path = relativeUrl.StartsWith("/", StringComparison.Ordinal)
            ? relativeUrl[1..]
            : relativeUrl;

        return new Uri(new Uri(normalizedBaseUri, UriKind.Absolute), path);
    }

    private Uri BuildUri(string relativeUrl)
        => BuildUri(_options.ApiBaseUrl, relativeUrl);

    private static T? TryDeserialize<T>(string json) where T : class
    {
        try
        {
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }
        catch
        {
            return null;
        }
    }

    private static string ResolveContentType(string fileName, string? contentType)
    {
        if (!string.IsNullOrWhiteSpace(contentType))
        {
            return contentType;
        }

        return Path.GetExtension(fileName).ToLowerInvariant() switch
        {
            ".funscript" => "application/json",
            ".json" => "application/json",
            ".csv" => "text/csv",
            _ => "application/octet-stream",
        };
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
