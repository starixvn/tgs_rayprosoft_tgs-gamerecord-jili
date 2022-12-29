using Tgs.GameCommand.Common;

namespace Tgs.GameRecord.JiliService.Core;

public interface IUtensil
{
    ValueTask<Guid?> BrandGetGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId);

    ValueTask<int?> GetGameIdAsync(Guid gameProviderId, string gameExternalId);

    ValueTask<Guid> GetGameProviderIdAsync(string gameProviderCode);

    ValueTask<PlayerInfo?> GetPlayerAsync(string playerCode);
}
