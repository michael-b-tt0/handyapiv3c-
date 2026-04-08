using System.Text.Json.Serialization;

namespace handyapiv3.Models;

public sealed class InfoResponse
{
    [JsonPropertyName("fw_status")]
    public int FirmwareStatus { get; set; }

    [JsonPropertyName("fw_version")]
    public string? FirmwareVersion { get; set; }

    [JsonPropertyName("fw_feature_flags")]
    public string? FirmwareFeatureFlags { get; set; }

    [JsonPropertyName("hw_model_no")]
    public int HardwareModelNo { get; set; }

    [JsonPropertyName("hw_model_name")]
    public string? HardwareModelName { get; set; }

    [JsonPropertyName("hw_model_variant")]
    public int HardwareModelVariant { get; set; }
}
