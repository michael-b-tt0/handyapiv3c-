using handyapiv3.Abstractions;
using handyapiv3.Services;
using Microsoft.Extensions.DependencyInjection;


namespace handyapiv3;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHandyApiV3(this IServiceCollection services, Action<HandyApiV3ClientOptions>? configure = null)
    {
        var options = new HandyApiV3ClientOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);
        services.AddScoped(sp =>
        {
            var httpClient = sp.GetRequiredService<HttpClient>();
            var configuredOptions = sp.GetRequiredService<HandyApiV3ClientOptions>();
            return new HandyApiV3Client(httpClient, configuredOptions);
        });
        services.AddScoped<IHandyService, HandyService>();

        return services;
    }
}
