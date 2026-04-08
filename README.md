# handyapiv3

A small .NET class library for working with The Handy REST API v3 from modern .NET apps such as Blazor WebAssembly.

This library provides two layers:

- `HandyApiV3Client`
  A thin, typed HTTP client for the Handy v3 REST endpoints.
- `IHandyService` / `HandyService`
  A higher-level, app-facing stateful service that is easier to use from UI code.

The service is designed to feel closer to a JS-style `new Handy()` object while still fitting naturally into .NET dependency injection.

## What This Library Covers

Current support includes:

- device connection status
- device info
- mode get/set
- HAMP start, stop, state, velocity
- HSSP setup, play, stop, state, sync time
- HSTP offset get/set
- slider stroke get/set
- server-time offset estimation

## Project Structure

- [HandyApiV3Client.cs](./HandyApiV3Client.cs)
  Raw HTTP client for the v3 API.
- [Services/HandyService.cs](./Services/HandyService.cs)
  Stateful app-facing service.
- [Abstractions/IHandyService.cs](./Abstractions/IHandyService.cs)
  Service contract for DI and testing.
- [Models](./Models)
  Request/response DTOs and shared models.
- [ServiceCollectionExtensions.cs](./ServiceCollectionExtensions.cs)
  Dependency injection registration helper.

## API Keys

The Handy v3 API uses two different identifiers:

- `ConnectionKey`
  The user/device-specific connection key sent as `X-Connection-Key`.
- `ApplicationApiKey`
  An application-level identifier sent as `X-Api-Key`.

Important:

- `ApplicationApiKey` should not be treated as a secret.
- In a Blazor WebAssembly app, it is sent from the browser and can be inspected by the user.
- `ConnectionKey` is the important device-specific credential.

By default, the library ships with a compatibility `ApplicationApiKey` value taken from the ScriptPlayer reference implementation. For a real application, you should prefer your own application identifier.

## Installation / Reference

Reference the class library from your application project in the usual way.

Example:

```xml
<ProjectReference Include="..\handyapiv3\handyapiv3.csproj" />
```

## Dependency Injection

Register the library in `Program.cs`:

```csharp
using handyapiv3;

builder.Services.AddHandyApiV3(options =>
{
    options.ConnectionKey = null;
    // options.ApplicationApiKey = "your-app-id";
    // options.ApiBaseUrl = "https://www.handyfeeling.com/api/handy-rest/v3/";
});
```

The registration expects a usable `HttpClient` to already exist in DI. In Blazor WebAssembly, that is typically already present.

## Recommended Usage Model

Most UI code should use `IHandyService`, not `HandyApiV3Client` directly.

Why:

- it keeps device state in one place
- it exposes a friendlier app-facing API
- it raises `StateChanged` when cached state changes
- it tracks values like `Connected`, `CurrentMode`, `Info`, `HampState`, and `EstimatedServerTimeOffset`

Use `HandyApiV3Client` directly only if you want raw endpoint access without state management.

## Example: Basic Usage

```csharp
@inject handyapiv3.Abstractions.IHandyService Handy

@code {
    private string? _firmware;
    private string? _error;

    protected override async Task OnInitializedAsync()
    {
        Handy.StateChanged += OnHandyStateChanged;

        try
        {
            Handy.SetConnectionKey("YOUR_CONNECTION_KEY");

            var info = await Handy.GetInfoAsync();
            _firmware = info.FirmwareVersion;
        }
        catch (Exception ex)
        {
            _error = ex.Message;
        }
    }

    private void OnHandyStateChanged(object? sender, EventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        Handy.StateChanged -= OnHandyStateChanged;
    }
}
```

## Example: HAMP

```csharp
Handy.SetConnectionKey(connectionKey);

await Handy.StartHampAsync();
await Handy.SetHampVelocityAsync(0.5);
await Handy.StopHampAsync();
```

Notes:

- `SetHampVelocityAsync` currently expects a normalized value in the range `0..1`.
- The service follows a v3-first approach and calls protocol endpoints directly rather than forcing an explicit mode switch first.

## Example: HSSP

```csharp
await Handy.SetupHsspFromUrlAsync(scriptUrl);

await Handy.EstimateServerTimeOffsetAsync();

await Handy.PlayHsspAsync(
    startTime: 0,
    playbackRate: 1.0,
    loop: false);
```

To perform a sync adjustment while playback is running:

```csharp
await Handy.SyncHsspTimeAsync(currentTime: 15000, filter: 1.0);
```

If you already have script content locally, v3 can push the content directly during setup:

```csharp
await Handy.SetupHsspFromCsvAsync(csvContent);
```

```csharp
await Handy.SetupHsspFromActionsJsonAsync(actionsJson);
```

`SetupHsspAsync(string scriptUrl)` is still available as a convenience alias for URL-based setup.

## Example: Slider Stroke

```csharp
await Handy.SetSliderStrokeAsync(0.2, 0.8);
var stroke = await Handy.GetSliderStrokeAsync();
```

Notes:

- `SetSliderStrokeAsync` expects normalized `0..1` values.
- If `min > max`, the values are automatically swapped.

## State Exposed By `IHandyService`

The service keeps a local cached view of the most useful device/application state:

- `ConnectionKey`
- `Connected`
- `CurrentMode`
- `Info`
- `HampState`
- `HsspState`
- `SliderStroke`
- `EstimatedServerTimeOffset`
- `LastError`

This state is updated when successful service calls complete, and `StateChanged` is raised afterward.

## Error Handling

If the Handy API returns an error envelope:

- the service updates `LastError`
- `Connected` is updated when the response provides connected-state information
- an `InvalidOperationException` is thrown

This means a common usage pattern is:

```csharp
try
{
    await Handy.GetInfoAsync();
}
catch (Exception ex)
{
    // show ex.Message or inspect Handy.LastError
}
```

## v3-First Behavior

This library is intentionally designed around Handy API v3 behavior.

In particular:

- HAMP and HSSP operations call their protocol endpoints directly
- explicit `SetModeAsync(...)` remains available, but is not required for normal HAMP/HSSP usage
- `CurrentMode` is updated from successful operations so the UI can still reflect mode changes

## Notes About the Reference Material

The `implementation material` folder in this project is kept as a local reference only.

- it is not compiled into the library
- it exists to preserve older ScriptPlayer and JS reference implementations while the new library evolves independently

## Current Limitations

- no script upload helper is included yet
- no HDSP support is implemented yet
- no persistence for `ConnectionKey` is built in
- no built-in throttling/queueing layer is included

Those can be added later depending on the needs of the host app.

## Suggested Next Steps

If you are integrating this into a Blazor WebAssembly app, a good next step is to add:

- a small UI component for connect/info/HAMP control
- connection key persistence in browser storage
- optional script upload support for HSSP workflows
