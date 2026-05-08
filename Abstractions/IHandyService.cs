using handyapiv3.Models;

namespace handyapiv3.Abstractions;

public interface IHandyService
{
    event EventHandler? StateChanged;

    string? ConnectionKey { get; }
    bool Connected { get; }
    HandyMode CurrentMode { get; }
    InfoResponse? Info { get; }
    HampStateResponse? HampState { get; }
    HdspStateResponse? HdspState { get; }
    HsspStateResponse? HsspState { get; }
    SliderStrokeResponse? SliderStroke { get; }
    long EstimatedServerTimeOffset { get; }
    HandyApiError? LastError { get; }
    int RateLimitPerMinute { get; }
    int RateLimitRemaining { get; }
    int SecondsUntilRateLimitReset { get; }

    void SetConnectionKey(string connectionKey);
    void ClearConnectionKey();

    Task<bool> GetConnectedAsync(CancellationToken cancellationToken = default);
    Task<InfoResponse> GetInfoAsync(CancellationToken cancellationToken = default);
    Task<HandyMode> GetModeAsync(CancellationToken cancellationToken = default);
    Task<ModeResponse> SetModeAsync(HandyMode mode, CancellationToken cancellationToken = default);
    Task<HampStateResponse> StartHampAsync(CancellationToken cancellationToken = default);
    Task<HampStateResponse> StopHampAsync(CancellationToken cancellationToken = default);
    Task<HampStateResponse> GetHampStateAsync(CancellationToken cancellationToken = default);
    Task<HampStateResponse> SetHampVelocityAsync(double velocity, CancellationToken cancellationToken = default);
    Task<string> SendHdspXavaAsync(double absolutePosition, double absoluteVelocity, bool stopOnTarget = false, bool immediateResponse = false, CancellationToken cancellationToken = default);
    Task<string> SendHdspXavpAsync(double absolutePosition, double percentVelocity, bool stopOnTarget = false, bool immediateResponse = false, CancellationToken cancellationToken = default);
    Task<string> SendHdspXpvaAsync(double normalizedPosition, double absoluteVelocity, bool stopOnTarget = false, bool immediateResponse = false, CancellationToken cancellationToken = default);
    Task<string> SendHdspXpvpAsync(double normalizedPosition, double percentVelocity, bool stopOnTarget = false, bool immediateResponse = false, CancellationToken cancellationToken = default);
    Task<string> SendHdspXatAsync(double absolutePosition, double duration, bool stopOnTarget = false, bool immediateResponse = false, CancellationToken cancellationToken = default);
    Task<string> SendHdspXptAsync(double normalizedPosition, double duration, bool stopOnTarget = false, bool immediateResponse = false, CancellationToken cancellationToken = default);
    Task<SliderStrokeResponse> GetSliderStrokeAsync(CancellationToken cancellationToken = default);
    Task<SliderStrokeResponse> SetSliderStrokeAsync(double min, double max, CancellationToken cancellationToken = default);
    Task<HsspStateResponse> GetHsspStateAsync(CancellationToken cancellationToken = default);
    Task<HsspStateResponse> SetupHsspAsync(string scriptUrl, CancellationToken cancellationToken = default);
    Task<HsspStateResponse> SetupHsspFromUrlAsync(string scriptUrl, CancellationToken cancellationToken = default);
    Task<ScriptUploadResponse> UploadScriptAsync(string fileName, byte[] content, string? contentType = null, CancellationToken cancellationToken = default);
    Task<ScriptUploadResponse> UploadScriptTextAsync(string fileName, string content, string? contentType = null, CancellationToken cancellationToken = default);
    
    Task<HsspStateResponse> PlayHsspAsync(long startTime, double playbackRate = 1.0, bool loop = false, long? serverTime = null, CancellationToken cancellationToken = default);
    Task<HsspStateResponse> PauseHsspAsync(CancellationToken cancellationToken = default);
    Task<HsspStateResponse> ResumeHsspAsync(bool pickUp = false, CancellationToken cancellationToken = default);
    Task<HsspStateResponse> StopHsspAsync(CancellationToken cancellationToken = default);
    Task<HsspStateResponse> SyncHsspTimeAsync(int currentTime, double filter = 1.0, long? serverTime = null, CancellationToken cancellationToken = default);
    Task<int> GetHstpOffsetAsync(CancellationToken cancellationToken = default);
    Task SetHstpOffsetAsync(int offset, CancellationToken cancellationToken = default);
    Task<long> EstimateServerTimeOffsetAsync(int trips = 30, CancellationToken cancellationToken = default);
    IAsyncEnumerable<HandySseEvent> SubscribeToDeviceEventsAsync(IEnumerable<string>? eventTypes = null, int? timeout = null, string? deviceReference = null, CancellationToken cancellationToken = default);
}
