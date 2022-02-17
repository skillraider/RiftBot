using Microsoft.Extensions.Configuration;

namespace RiftBot.Types.Utilities;

public static class ConfigurationExtensions
{
    public static string GetApiEndpoints(this IConfiguration configuration, string name)
    {
        return configuration?.GetSection("ApiEndpoints")?[name];
    }
}