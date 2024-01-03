using System.Reflection;
using StackExchange.Redis;
using Tgs.GameCommand.Client;
using Tgs.GameInterface.Jili.Client;
using Tgs.GameRecord.Jili.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameRecordCore(
        this IServiceCollection services,
        Action<GameCommandClientOptions, IServiceProvider> gameCommandClientOptions,
        Action<GameInterfaceJiliClientOptions, IServiceProvider> gameInterfaceJiliClientOptions,
        Func<IServiceProvider, IDatabase> redisDatabaseResolver)
        => services
        .RegisterAllServiceUtilityTypes(
            Assembly.GetExecutingAssembly(),
            ServiceLifetime.Transient)
        .AddGameCommandClient(gameCommandClientOptions)
        .AddGameInterfaceJiliServiceClient(gameInterfaceJiliClientOptions)
        .AddGameRecordHandleClient()
        .AddTransient<IHangfireUtensil, HangfireUtensil>()
        .AddTransient<IHangfireLock>(sp => ActivatorUtilities.CreateInstance<HangfireLock>(sp, redisDatabaseResolver(sp)));
}
