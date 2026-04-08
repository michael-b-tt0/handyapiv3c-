namespace handyapiv3;

public sealed class HandyApiV3ClientOptions
{
    public const string DefaultApiBaseUrl = "https://www.handyfeeling.com/api/handy-rest/v3/";
    public const string DefaultApplicationApiKey = "wKTdv0fJNfBRdkf4-x5gUvtSuNPWzv-s";

    public string ApiBaseUrl { get; set; } = DefaultApiBaseUrl;
    public string ApplicationApiKey { get; set; } = DefaultApplicationApiKey;
    public string? ConnectionKey { get; set; }
}
