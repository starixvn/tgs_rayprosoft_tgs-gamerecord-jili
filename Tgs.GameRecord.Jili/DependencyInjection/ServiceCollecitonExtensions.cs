using Tgs.GameCommand.Client;
using Tgs.GameInterface.Jili.Client;
using Tgs.GameRecord.Jili;
using Tgs.GameRecord.Jili.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
    public static IServiceCollection AddGameRecord(
        this IServiceCollection services,
        Action<RedisOptions, IServiceProvider> redisDBconfigure,
        Action<GameRecordInfoOptions, IServiceProvider> gameRecordInfoOptions,
        Action<GameCommandClientOptions, IServiceProvider> gameCommandClientOptions,
        Action<GameInterfaceJiliClientOptions, IServiceProvider> gameInterfaceJiliClientOptions)
        => services
        .AddOptions<RedisOptions>()
        .Configure(redisDBconfigure)
        .Services
        .AddOptions<GameRecordInfoOptions>()
        .Configure(gameRecordInfoOptions)
        .Services
        .AddSingleton<RedisConnectionManager>()
        .AddGameRecordCore(
            gameCommandClientOptions: gameCommandClientOptions,
            gameInterfaceJiliClientOptions: gameInterfaceJiliClientOptions,
            redisDatabaseResolver: (sp) => sp.GetRequiredService<RedisConnectionManager>().Database);
}
