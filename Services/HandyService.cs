using handyapiv3.Abstractions;
using handyapiv3.Models;
using System.Text;

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

    public int RateLimitPerMinute { get; private set; }

    public int RateLimitRemaining { get; private set; }

    public int SecondsUntilRateLimitReset { get; private set; }

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
        RateLimitPerMinute = 0;
        RateLimitRemaining = 0;
        SecondsUntilRateLimitReset = 0;
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

    public async Task<string> SendHdspXavaAsync(double absolutePosition, double absoluteVelocity, bool stopOnTarget = false, bool immediateResponse = false, CancellationToken cancellationToken = default)
    {
        var response = await _client.PutHdspXavaAsync(new HdspXavaRequest
        {
            AbsolutePosition = Math.Max(0, absolutePosition),
            AbsoluteVelocity = Math.Max(0, absoluteVelocity),
            StopOnTarget = stopOnTarget,
            ImmediateResponse = immediateResponse,
        }, cancellationToken);

        return HandleHdspCommand(response);
    }

    public async Task<string> SendHdspXavpAsync(double absolutePosition, double percentVelocity, bool stopOnTarget = false, bool immediateResponse = false, CancellationToken cancellationToken = default)
    {
        var response = await _client.PutHdspXavpAsync(new HdspXavpRequest
        {
            AbsolutePosition = Math.Max(0, absolutePosition),
            PercentVelocity = Math.Clamp(percentVelocity, 0, 100),
            StopOnTarget = stopOnTarget,
            ImmediateResponse = immediateResponse,
        }, cancellationToken);

        return HandleHdspCommand(response);
    }

    public async Task<string> SendHdspXpvaAsync(double normalizedPosition, double absoluteVelocity, bool stopOnTarget = false, bool immediateResponse = false, CancellationToken cancellationToken = default)
    {
        var response = await _client.PutHdspXpvaAsync(new HdspXpvaRequest
        {
            NormalizedPosition = Math.Clamp(normalizedPosition, 0, 1),
            AbsoluteVelocity = Math.Max(0, absoluteVelocity),
            StopOnTarget = stopOnTarget,
            ImmediateResponse = immediateResponse,
        }, cancellationToken);

        return HandleHdspCommand(response);
    }

    public async Task<string> SendHdspXpvpAsync(double normalizedPosition, double percentVelocity, bool stopOnTarget = false, bool immediateResponse = false, CancellationToken cancellationToken = default)
    {
        var response = await _client.PutHdspXpvpAsync(new HdspXpvpRequest
        {
            NormalizedPosition = Math.Clamp(normalizedPosition, 0, 1),
            PercentVelocity = Math.Clamp(percentVelocity, 0, 100),
            StopOnTarget = stopOnTarget,
            ImmediateResponse = immediateResponse,
        }, cancellationToken);

        return HandleHdspCommand(response);
    }

    public async Task<string> SendHdspXatAsync(double absolutePosition, double duration, bool stopOnTarget = false, bool immediateResponse = false, CancellationToken cancellationToken = default)
    {
        var response = await _client.PutHdspXatAsync(new HdspXatRequest
        {
            AbsolutePosition = Math.Max(0, absolutePosition),
            Duration = Math.Max(0, duration),
            StopOnTarget = stopOnTarget,
            ImmediateResponse = immediateResponse,
        }, cancellationToken);

        return HandleHdspCommand(response);
    }

    public async Task<string> SendHdspXptAsync(double normalizedPosition, double duration, bool stopOnTarget = false, bool immediateResponse = false, CancellationToken cancellationToken = default)
    {
        var response = await _client.PutHdspXptAsync(new HdspXptRequest
        {
            NormalizedPosition = Math.Clamp(normalizedPosition, 0, 1),
            Duration = Math.Max(0, duration),
            StopOnTarget = stopOnTarget,
            ImmediateResponse = immediateResponse,
        }, cancellationToken);

        return HandleHdspCommand(response);
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

    public Task<ScriptUploadResponse> UploadScriptAsync(string fileName, byte[] content, string? contentType = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("A file name is required.", nameof(fileName));
        }

        if (content.Length == 0)
        {
            throw new ArgumentException("Script content is required.", nameof(content));
        }

        return _client.UploadScriptAsync(fileName, content, contentType, cancellationToken);
    }

    public Task<ScriptUploadResponse> UploadScriptTextAsync(string fileName, string content, string? contentType = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Script content is required.", nameof(content));
        }

        return UploadScriptAsync(fileName, Encoding.UTF8.GetBytes(content), contentType, cancellationToken);
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

    public async Task<HsspStateResponse> PauseHsspAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.PauseHsspAsync(cancellationToken);
        return Handle(response, result =>
        {
            HsspState = result;
            CurrentMode = HandyMode.Hssp;
            return result;
        });
    }

    public async Task<HsspStateResponse> ResumeHsspAsync(bool pickUp = false, CancellationToken cancellationToken = default)
    {
        var response = await _client.ResumeHsspAsync(new HsspResumeRequest
        {
            PickUp = pickUp,
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

    public async Task<long> EstimateServerTimeOffsetAsync(int trips = 30, CancellationToken cancellationToken = default)
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

    private string HandleHdspCommand(HandyApiResponse<string> response)
        => Handle(response, result =>
        {
            CurrentMode = HandyMode.Hdsp;
            return result;
        });

    private TOut Handle<TResult, TOut>(HandyApiResponse<TResult> response, Func<TResult, TOut> onSuccess) where TResult : class
    {
        UpdateRateLimitState(response);

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

    private void UpdateRateLimitState(HandyApiResponse response)
    {
        RateLimitPerMinute = response.RateLimitPerMinute;
        RateLimitRemaining = response.RateLimitRemaining;
        SecondsUntilRateLimitReset = response.SecondsUntilRateLimitReset;
    }

    private void OnStateChanged() => StateChanged?.Invoke(this, EventArgs.Empty);
}
