using System.Text.Json;
using System.Text.Json.Serialization;

namespace handyapiv3.Models;

public static class HandySseEventTypes
{
    public const string BatteryChanged = "battery_changed";
    public const string BleStatusChanged = "ble_status_changed";
    public const string ButtonEvent = "button_event";
    public const string DeviceClockSynchronized = "device_clock_synchronized";
    public const string DeviceConnected = "device_connected";
    public const string DeviceDisconnected = "device_disconnected";
    public const string DeviceError = "device_error";
    public const string DeviceStatus = "device_status";
    public const string HampStateChanged = "hamp_state_changed";
    public const string HrppStateChanged = "hrpp_state_changed";
    public const string HdspStateChanged = "hdsp_state_changed";
    public const string HspLooping = "hsp_looping";
    public const string HspStarving = "hsp_starving";
    public const string HspStateChanged = "hsp_state_changed";
    public const string HspThresholdReached = "hsp_threshold_reached";
    public const string HspPausedOnStarving = "hsp_paused_on_starving";
    public const string HspResumedOnNotStarving = "hsp_resumed_on_not_starving";
    public const string StreamEndReached = "stream_end_reached";
    public const string HvpStateChanged = "hvp_state_changed";
    public const string LowMemoryError = "low_memory_error";
    public const string LowMemoryWarning = "low_memory_warning";
    public const string ModeChanged = "mode_changed";
    public const string OtaProgress = "ota_progress";
    public const string SliderBlocked = "slider_blocked";
    public const string SliderUnblocked = "slider_unblocked";
    public const string StrokeChanged = "stroke_changed";
    public const string TempHigh = "temp_high";
    public const string TempOk = "temp_ok";
    public const string WifiScanComplete = "wifi_scan_complete";
    public const string WifiStatus = "wifi_status";
    public const string WifiStatusChanged = "wifi_status_changed";
}

public sealed class HandySseEvent
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("data")]
    public HandySseEventPayload? Payload { get; set; }

    public T? DeserializeDeviceData<T>() where T : class
        => Payload?.DeserializeData<T>();
}

public sealed class HandySseEventPayload
{
    [JsonPropertyName("connection_key")]
    public string? ConnectionKey { get; set; }

    [JsonPropertyName("data")]
    public JsonElement Data { get; set; }

    public T? DeserializeData<T>() where T : class
    {
        if (Data.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
        {
            return null;
        }

        return JsonSerializer.Deserialize<T>(Data.GetRawText(), HandySseJson.Options);
    }
}

public sealed class DeviceStatusEventData
{
    [JsonPropertyName("connected")]
    public bool Connected { get; set; }

    [JsonPropertyName("info")]
    public InfoResponse? Info { get; set; }
}

public sealed class DeviceDisconnectedEventData
{
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    [JsonPropertyName("session_id")]
    public string? SessionId { get; set; }

    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; set; }
}

public sealed class BatteryStatusEventData
{
    [JsonPropertyName("level")]
    public double Level { get; set; }

    [JsonPropertyName("charger_connected")]
    public bool ChargerConnected { get; set; }

    [JsonPropertyName("charging_complete")]
    public bool ChargingComplete { get; set; }

    [JsonPropertyName("usb_voltage")]
    public double UsbVoltage { get; set; }

    [JsonPropertyName("battery_voltage")]
    public double BatteryVoltage { get; set; }

    [JsonPropertyName("usb_adc_value")]
    public int UsbAdcValue { get; set; }

    [JsonPropertyName("battery_adc_value")]
    public int BatteryAdcValue { get; set; }
}

public sealed class ButtonEventData
{
    [JsonPropertyName("button")]
    public int Button { get; set; }

    [JsonPropertyName("event")]
    public int Event { get; set; }
}

public sealed class WifiStatusEventData
{
    [JsonPropertyName("socket_connected")]
    public bool SocketConnected { get; set; }

    [JsonPropertyName("state")]
    public int State { get; set; }
}

public sealed class BleStatusEventData
{
    [JsonPropertyName("status")]
    public int Status { get; set; }
}

public sealed class WifiScanCompleteEventData
{
    [JsonPropertyName("nr_of_networks")]
    public int NumberOfNetworks { get; set; }
}

public sealed class LowMemoryWarningEventData
{
    [JsonPropertyName("available_heap")]
    public int AvailableHeap { get; set; }

    [JsonPropertyName("larges_free_block")]
    public int LargestFreeBlock { get; set; }
}

public sealed class LowMemoryErrorEventData
{
    [JsonPropertyName("available_heap")]
    public int AvailableHeap { get; set; }

    [JsonPropertyName("larges_free_block")]
    public int LargestFreeBlock { get; set; }

    [JsonPropertyName("discarded_msg_size")]
    public int DiscardedMessageSize { get; set; }
}

public sealed class ErrorEventData
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }
}

public sealed class OtaProgressEventData
{
    [JsonPropertyName("done")]
    public bool Done { get; set; }

    [JsonPropertyName("progress")]
    public double Progress { get; set; }

    [JsonPropertyName("failed")]
    public bool Failed { get; set; }
}

internal static class HandySseJson
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
    };
}
