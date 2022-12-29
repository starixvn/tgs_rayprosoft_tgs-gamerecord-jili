using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;

namespace Tgs.GameRecord.JiliService.Core;

internal class UtensilRepository : IUtensil
{
    private readonly IGameCommandClient _client;

    public UtensilRepository(
        IGameCommandClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public ValueTask<Guid?> BrandGetGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId)
        => _client.BrandGetGameProviderConfigurationIdAsync(brandId, gameProviderId);

    public async ValueTask<int?> GetGameIdAsync(Guid gameProviderId, string gameExternalId)
    {
        var gameModel = await _client.GameGetAsync(gameProviderId, gameExternalId).ConfigureAwait(false);
        return gameModel?.GameId;
    }

    public async ValueTask<Guid> GetGameProviderIdAsync(string gameProviderCode)
    => await _client.GameProviderGetGameProviderIdAsync(gameProviderCode).ConfigureAwait(false)
        ?? throw new Exception($"The identifier for this game provider does not exist.({gameProviderCode})");

    public ValueTask<PlayerInfo?> GetPlayerAsync(string playerCode)
        => _client.PlayerGetAsync(playerCode);
}
