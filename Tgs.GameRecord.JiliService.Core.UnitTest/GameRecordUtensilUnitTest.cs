using Google.Protobuf.WellKnownTypes;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.JiliService.Protos;

namespace Tgs.GameRecord.JiliService.Core.UnitTest;

public class GameRecordUtensilUnitTest
{
    [Fact]
    public async Task Settlement_Normal_Process()
    {
        var fakeUtensil = Substitute.For<IUtensil>();

        var source = new GamePullRecord
        {
            Account = "4uiuj5ovw7zk",
            WagersId = "1680840597057173115",
            GameId = 115,
            WagersTime = DateTimeOffset.Parse("2023-04-17T21:40:53-04:00").ToTimestamp(),
            BetAmount = -1,
            PayoffTime = DateTimeOffset.Parse("2023-04-17T21:40:53-04:00").ToTimestamp(),
            PayoffAmount = 0.8m,
            Status = (GamePullRecordStatus)2,
            SettlementTime = DateTimeOffset.Parse("2023-04-17T21:40:53-04:00").ToTimestamp(),
            GameCategoryId = 1,
            Type = 1,
            AgentId = "PG_GPI_CNY",
            Turnover = 1,
            AdditionalData = "{\"Account\":\"4uiuj5ovw7zk\",\"WagersId\":1680840597057173115,\"GameId\":115,\"WagersTime\":\"2023-04-17T21:40:53-04:00\",\"BetAmount\":-1,\"PayoffTime\":\"2023-04-17T21:40:53-04:00\",\"PayoffAmount\":0.8,\"Status\":2,\"SettlementTime\":\"2023-04-17T21:40:53-04:00\",\"GameCategoryId\":1,\"VersionKey\":0,\"Type\":1,\"AgentId\":\"PG_GPI_CNY\",\"Turnover\":1}"
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeUtensil
            .GetGameProviderIdAsync(
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var gameProviderCode = "JILITF";
        _ = fakeUtensil
            .GetGameProviderCode()
            .Returns(gameProviderCode);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "PB2",
            PlayerCode: source.Account,
            Username: "test",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "PHP"
            );
        _ = fakeUtensil
            .GetPlayerAsync(
                playerCode: Arg.Is(source.Account),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(player);

        var gameId = 1;
        _ = fakeUtensil
            .GetGameIdAsync(
                gameProviderId: Arg.Is(gameProviderId),
                gameExternalId: Arg.Is(source.GameId.ToString()),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameId);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeUtensil
            .GetBrandGameProviderConfigurationIdAsync(
                brandId: Arg.Is(player.BrandId),
                gameProviderId: Arg.Is(gameProviderId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var gameRecordUtensil = new GameRecordUtensil(fakeUtensil);

        var actual = await gameRecordUtensil
             .SettlementConvertRecordAsync(
                 source: source)
             .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.Equal(gameProviderId, actual.GameProviderId);
        Assert.Equal(gameProviderCode, actual.GameProviderCode);
        Assert.Equal(source.WagersId, actual.ExternalRoundId);
        Assert.Equal(gameId, actual.GameId);
        Assert.Equal(source.GameId.ToString(), actual.ExternalGameId);
        Assert.Equal(source.Account, actual.PlayerCode);
        Assert.Equal(player.DefaultWalletId, actual.WalletId);
        Assert.Equal(player.DefaultWalletCurrencyCode, actual.CurrencyCode);
        Assert.Equal(player.BrandId, actual.BrandId);
        Assert.Equal(gameProviderConfigurationId, actual.GameProviderConfigurationId);
        Assert.Equal(source.WagersTime.ToDateTimeOffset(), actual.ExternalCreatedOn);
        Assert.Equal(source.PayoffTime.ToDateTimeOffset(), actual.ExternalClosedOn);
        Assert.Equal(source.BetAmount.ToDecimal().GetValueOrDefault(0), actual.Bet);
        Assert.Equal(source.Turnover.ToDecimal().GetValueOrDefault(0), actual.ValidBet);
        Assert.Equal(source.Turnover.ToDecimal().GetValueOrDefault(0), actual.Turnover);
        Assert.Equal(source.PayoffAmount.ToDecimal().GetValueOrDefault(0) - source.BetAmount.ToDecimal().GetValueOrDefault(0), actual.WinLoss);
        Assert.Equal(source.PayoffAmount.ToDecimal().GetValueOrDefault(0), actual.Settle);
        Assert.Equal(source.AdditionalData, actual.AdditionalData);
    }
}
