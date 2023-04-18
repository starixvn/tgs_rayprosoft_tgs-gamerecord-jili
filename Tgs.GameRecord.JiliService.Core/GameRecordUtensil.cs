using Tgs.GameInterface.JiliService.Protos;
using Tgs.GameRecord.JiliService.Core.Models;

namespace Tgs.GameRecord.JiliService.Core;

internal class GameRecordUtensil : IGameRecordUtensil
{
    private readonly IUtensil _utensil;

    public GameRecordUtensil(
        IUtensil utensil)
    {
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
    }

    public async ValueTask<RecordInfo> SettlementConvertRecordAsync(
        GamePullRecord source,
        CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _utensil
            .GetGameProviderIdAsync(
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderCode = _utensil
            .GetGameProviderCode();

        bool success = true;

        var player = await _utensil
            .GetPlayerAsync(
                playerCode: source.Account,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameId = await _utensil
            .GetGameIdAsync(
                gameProviderId: gameProviderId,
                gameExternalId: source.GameId.ToString(),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (player == null || !gameId.HasValue)
        {
            success = false;
        }

        Guid? gameProviderConfigurationId = default;
        if (player != null)
        {
            gameProviderConfigurationId = await _utensil
                .GetBrandGameProviderConfigurationIdAsync(
                    brandId: player.BrandId,
                    gameProviderId: gameProviderId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            if (!gameProviderConfigurationId.HasValue)
            {
                success = false;
            }
        }
        var externalCreatedOn = source.WagersTime.ToDateTimeOffset();
        var externalClosedOn = source.PayoffTime.ToDateTimeOffset();
        var bet = source.BetAmount.ToDecimal().GetValueOrDefault(0);
        var validBet = source.Turnover.ToDecimal().GetValueOrDefault(0);
        var settle = source.PayoffAmount.ToDecimal().GetValueOrDefault(0);
        var commission = 0;
        var turnover = source.Turnover.ToDecimal().GetValueOrDefault(0);
        var winLoss = source.PayoffAmount.ToDecimal().GetValueOrDefault(0) - source.BetAmount.ToDecimal().GetValueOrDefault(0);

        return new RecordInfo(
            Success: success,
            GameProviderId: gameProviderId,
            GameProviderCode: gameProviderCode,
            ExternalRoundId: source.WagersId,
            GameId: gameId ?? -1,
            ExternalGameId: source.GameId.ToString(),
            PlayerCode: (player == null) ? string.Empty : player.PlayerCode,
            WalletId: (player == null) ? Guid.Empty : player.DefaultWalletId,
            CurrencyCode: (player == null) ? string.Empty : player.DefaultWalletCurrencyCode,
            BrandId: (player == null) ? Guid.Empty : player.BrandId,
            GameProviderConfigurationId: gameProviderConfigurationId ?? Guid.Empty,
            Status: source.Status,
            ExternalCreatedOn: externalCreatedOn,
            ExternalClosedOn: externalClosedOn,
            Bet: bet,
            ValidBet: validBet,
            Settle: settle,
            Commission: commission,
            Turnover: turnover,
            WinLoss: winLoss,
            AdditionalData: source.AdditionalData);
    }
}
