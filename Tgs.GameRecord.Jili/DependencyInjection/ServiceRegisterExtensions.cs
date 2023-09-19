using Tgs.GameInterface.Jili.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceRegisterExtensions
{
    public static async Task<WebApplication> UseRegister(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var client = scope.ServiceProvider.GetRequiredService<IGameInterfaceJiliClient>();

            _ = await client
                .CheckingServiceAsync(
                machineName: Environment.MachineName)
                .ConfigureAwait(false);
        }

        return app;
    }
}
