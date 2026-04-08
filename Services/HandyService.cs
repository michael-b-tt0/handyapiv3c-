using handyapiv3.Abstractions;
using handyapiv3.Models;

namespace handyapiv3.Services;

public sealed class HandyService : IHandyService
{
    private readonly HandyApiV3Client _client;

    public HandyService(HandyApiV3Client client)
    {
        _client = client;
    }

    public event EventHandler? StateChanged;

    public string? ConnectionKey => _client.ConnectionKey;

    public bool Connected { get; private set; }

    public HandyMode CurrentMode { get; private set; }

    public InfoResponse? Info { get; private set; }

    public HampStateResponse? HampState { get; private set; }

    public HsspStateResponse? HsspState { get; private set; }

    public SliderStrokeResponse? SliderStroke { get; private set; }

    public long EstimatedServerTimeOffset { get; private set; }

    public HandyApiError? LastError { get; private set; }

    public void SetConnectionKey(string connectionKey)
    {
        _client.ConnectionKey = connectionKey;
        OnStateChanged();
    }

    public void ClearConnectionKey()
    {
        _client.ConnectionKey = null;
        Connected = false;
        LastError = null;
        OnStateChanged();
    }

    public async Task<bool> GetConnectedAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.GetConnectedAsync(cancellationToken);
        return Handle(response, result =>
        {
            Connected = result.Connected;
            return result.Connected;
        });
    }

    public async Task<InfoResponse> GetInfoAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.GetInfoAsync(cancellationToken);
        return Handle(response, result =>
        {
            Info = result;
            return result;
        });
    }

    public async Task<HandyMode> GetModeAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.GetModeAsync(cancellationToken);
        return Handle(response, result =>
        {
            CurrentMode = (HandyMode)result.Mode;
            return CurrentMode;
        });
    }

    public async Task<ModeResponse> SetModeAsync(HandyMode mode, CancellationToken cancellationToken = default)
    {
        var response = await _client.PutModeAsync(mode, cancellationToken);
        return Handle(response, result =>
        {
            CurrentMode = (HandyMode)result.Mode;
            return result;
        });
    }

    public async Task<HampStateResponse> StartHampAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.StartHampAsync(cancellationToken);
        return Handle(response, result =>
        {
            HampState = result;
            CurrentMode = HandyMode.Hamp;
            return result;
        });
    }

    public async Task<HampStateResponse> StopHampAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.StopHampAsync(cancellationToken);
        return Handle(response, result =>
        {
            HampState = result;
            CurrentMode = HandyMode.Hamp;
            return result;
        });
    }

    public async Task<HampStateResponse> GetHampStateAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.GetHampStateAsync(cancellationToken);
        return Handle(response, result =>
        {
            HampState = result;
            CurrentMode = HandyMode.Hamp;
            return result;
        });
    }

    public async Task<HampStateResponse> SetHampVelocityAsync(double velocity, CancellationToken cancellationToken = default)
    {
        velocity = Math.Clamp(velocity, 0, 1);

        var response = await _client.PutHampVelocityAsync(velocity, cancellationToken);
        return Handle(response, result =>
        {
            HampState = result;
            CurrentMode = HandyMode.Hamp;
            return result;
        });
    }

    public async Task<SliderStrokeResponse> GetSliderStrokeAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.GetSliderStrokeAsync(cancellationToken);
        return Handle(response, result =>
        {
            SliderStroke = result;
            return result;
        });
    }

    public async Task<SliderStrokeResponse> SetSliderStrokeAsync(double min, double max, CancellationToken cancellationToken = default)
    {
        min = Math.Clamp(min, 0, 1);
        max = Math.Clamp(max, 0, 1);

        if (min > max)
        {
            (min, max) = (max, min);
        }

        var response = await _client.PutSliderStrokeAsync(new SliderSettings
        {
            Min = min,
            Max = max,
        }, cancellationToken);

        return Handle(response, result =>
        {
            SliderStroke = result;
            return result;
        });
    }

    public async Task<HsspStateResponse> GetHsspStateAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.GetHsspStateAsync(cancellationToken);
        return Handle(response, result =>
        {
            HsspState = result;
            CurrentMode = HandyMode.Hssp;
            return result;
        });
    }

    public async Task<HsspStateResponse> SetupHsspAsync(string scriptUrl, CancellationToken cancellationToken = default)
        => await SetupHsspFromUrlAsync(scriptUrl, cancellationToken);

    public async Task<HsspStateResponse> SetupHsspFromUrlAsync(string scriptUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(scriptUrl))
        {
            throw new ArgumentException("A script URL is required.", nameof(scriptUrl));
        }

        var response = await _client.SetupHsspAsync(new HsspSetupUrlRequest { Url = scriptUrl }, cancellationToken);
        return Handle(response, result =>
        {
            HsspState = result;
            CurrentMode = HandyMode.Hssp;
            return result;
        });
    }

    public async Task<HsspStateResponse> SetupHsspFromCsvAsync(string csvContent, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(csvContent))
        {
            throw new ArgumentException("CSV content is required.", nameof(csvContent));
        }

        var response = await _client.SetupHsspAsync(new HsspSetupCsvRequest { Csv = csvContent }, cancellationToken);
        return Handle(response, result =>
        {
            HsspState = result;
            CurrentMode = HandyMode.Hssp;
            return result;
        });
    }

    public async Task<HsspStateResponse> SetupHsspFromActionsJsonAsync(string actionsJson, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(actionsJson))
        {
            throw new ArgumentException("Actions JSON is required.", nameof(actionsJson));
        }

        var response = await _client.SetupHsspAsync(new HsspSetupActionsJsonRequest { Actions = actionsJson }, cancellationToken);
        return Handle(response, result =>
        {
            HsspState = result;
            CurrentMode = HandyMode.Hssp;
            return result;
        });
    }

    public async Task<HsspStateResponse> PlayHsspAsync(long startTime, double playbackRate = 1.0, bool loop = false, long? serverTime = null, CancellationToken cancellationToken = default)
    {
        var response = await _client.PlayHsspAsync(new HsspPlayRequest
        {
            StartTime = Math.Max(0, startTime),
            ServerTime = serverTime ?? GetEstimatedServerTime(),
            PlaybackRate = playbackRate,
            Loop = loop,
        }, cancellationToken);

        return Handle(response, result =>
        {
            HsspState = result;
            CurrentMode = HandyMode.Hssp;
            return result;
        });
    }

    public async Task<HsspStateResponse> StopHsspAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.StopHsspAsync(cancellationToken);
        return Handle(response, result =>
        {
            HsspState = result;
            CurrentMode = HandyMode.Hssp;
            return result;
        });
    }

    public async Task<HsspStateResponse> SyncHsspTimeAsync(int currentTime, double filter = 1.0, long? serverTime = null, CancellationToken cancellationToken = default)
    {
        var response = await _client.SyncHsspTimeAsync(new HsspSyncTimeRequest
        {
            CurrentTime = Math.Max(0, currentTime),
            ServerTime = serverTime ?? GetEstimatedServerTime(),
            Filter = filter,
        }, cancellationToken);

        return Handle(response, result =>
        {
            HsspState = result;
            CurrentMode = HandyMode.Hssp;
            return result;
        });
    }

    public async Task<int> GetHstpOffsetAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.GetHstpOffsetAsync(cancellationToken);
        return Handle(response, result => result.Offset);
    }

    public async Task SetHstpOffsetAsync(int offset, CancellationToken cancellationToken = default)
    {
        var response = await _client.PutHstpOffsetAsync(new HstpOffsetRequest { Offset = offset }, cancellationToken);
        Handle(response, _ => true);
    }

    public async Task<long> EstimateServerTimeOffsetAsync(int trips = 10, CancellationToken cancellationToken = default)
    {
        trips = Math.Max(1, trips);
        await _client.GetServerTimeAsync(cancellationToken);

        var offsets = new List<long>(trips);

        for (var i = 0; i < trips; i++)
        {
            var started = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var serverTime = await _client.GetServerTimeAsync(cancellationToken);
            var finished = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var roundTrip = finished - started;
            var estimatedServerTime = serverTime.ServerTime + (roundTrip / 2);
            offsets.Add(estimatedServerTime - finished);
        }

        EstimatedServerTimeOffset = (long)Math.Round(offsets.Average());
        OnStateChanged();
        return EstimatedServerTimeOffset;
    }

    private long GetEstimatedServerTime()
        => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + EstimatedServerTimeOffset;

    private TOut Handle<TResult, TOut>(HandyApiResponse<TResult> response, Func<TResult, TOut> onSuccess) where TResult : class
    {
        if (response.Error is not null)
        {
            LastError = response.Error;

            if (response.Error.Connected.HasValue)
            {
                Connected = response.Error.Connected.Value;
            }

            OnStateChanged();
            throw new InvalidOperationException(response.Error.Message ?? response.Error.Name ?? "Handy API returned an error.");
        }

        LastError = null;
        Connected = true;
        var result = onSuccess(response.Result ?? throw new InvalidOperationException("Handy API returned no result."));
        OnStateChanged();
        return result;
    }

    private void OnStateChanged() => StateChanged?.Invoke(this, EventArgs.Empty);
}
