using System.Reflection;
using StackExchange.Redis;
using Tgs.GameInterface.JiliService.Client;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameRecord.JiliService.Core;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJiliServiceCore(
            this IServiceCollection services,
            Action<GameRecordInfoOptions, IServiceProvider> gameRecordInfoOptions,
            Action<GameInterfaceJiliServiceClientOptions, IServiceProvider> gameInterfaceJiliServiceClientOptions,
            Action<GameProvidersQueryServerClientOptions, IServiceProvider> gameProvidersQueryServerClientOptions,
            Func<IServiceProvider, IDatabase> redisDatabaseResolver
            )
            => services
                .AddOptions<GameRecordInfoOptions>()
                .Configure(gameRecordInfoOptions)
                .Services
                .RegisterAllServiceUtilityTypes(
                    Assembly.GetExecutingAssembly(),
                    ServiceLifetime.Transient)
                .AddGameInterfaceJiliServiceClient(gameInterfaceJiliServiceClientOptions)
                .AddGameProvidersQueryServerClient(gameProvidersQueryServerClientOptions)
                .AddTransient<IScheduleDispatch>(sp => ActivatorUtilities.CreateInstance<ScheduleDispatch>(sp, redisDatabaseResolver(sp), TimeSpan.FromMinutes(4)))
                .AddTransient<IUtensil, UtensilRepository>()
                .Decorate<IUtensil>((innerRepository, sp) => ActivatorUtilities.CreateInstance<UtensilMemoryCacheRepository>(sp, innerRepository, TimeSpan.FromHours(1)));
    }
}
