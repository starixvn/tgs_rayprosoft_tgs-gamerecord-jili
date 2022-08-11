using Tgs.GameRecord.JiliService;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameRecordJiliService(
        this IServiceCollection services,
        Action<RedisOptions, IServiceProvider> redisDBconfigure)
        => services
            .AddOptions<RedisOptions>()
            .Configure(redisDBconfigure)
            .Services
            .AddSingleton<RedisConnectionManager>();
}
