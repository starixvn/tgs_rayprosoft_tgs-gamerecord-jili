using System.Text.Json;
using NSubstitute;
using Tgs.GameInterface.JiliService.Protos;
using Tgs.GameRecord.JiliService.Core.ApplicationServices;
using Tgs.GameRecord.JiliService.Core.Models;
using Tgs.GameRounds.Work.Client;
using Tgs.GameRounds.Work.Common.Protos;

namespace Tgs.GameRecord.JiliService.Core.UnitTest.ApplicationServices;

public class HandleRecordCommandServiceUnitTest
{
    [Fact]
    public async void Normal_Process_Settlement_Loss()
    {
        var fakeClient = Substitute.For<IGameRoundsWorkClient>();

        var command = new HandleRecordCommand(
            Record: new RecordInfo(
                Success: true,
                GameProviderId: Guid.NewGuid(),
                GameProviderCode: "JILITF",
                ExternalRoundId: "1680840597057153115",
                GameId: 1,
                ExternalGameId: "115",
                PlayerCode: "4uiuj5ovw7zk",
                WalletId: Guid.NewGuid(),
                CurrencyCode: "CNY",
                BrandId: Guid.NewGuid(),
                GameProviderConfigurationId: Guid.NewGuid(),
                Status: (GamePullRecordStatus)2,
                ExternalCreatedOn: DateTimeOffset.Parse("2023-04-17T21:02:09-04:00"),
                ExternalClosedOn: DateTimeOffset.Parse("2023-04-17T21:02:09-04:00"),
                Bet: 1,
                ValidBet: 1,
                Settle: 0,
                Commission: 0,
                Turnover: 1,
                WinLoss: -1m,
                AdditionalData: "{\"Account\":\"4uiuj5ovw7zk\",\"WagersId\":1680840597057153115,\"GameId\":115,\"WagersTime\":\"2023-04-17T21:02:09-04:00\",\"BetAmount\":-1,\"PayoffTime\":\"2023-04-17T21:02:09-04:00\",\"PayoffAmount\":0,\"Status\":2,\"SettlementTime\":\"2023-04-17T21:02:09-04:00\",\"GameCategoryId\":1,\"VersionKey\":0,\"Type\":1,\"AgentId\":\"PG_GPI_CNY\",\"Turnover\":1}"
                ));

        var service = new HandleRecordCommandService(fakeClient);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        await fakeClient
            .Received(1)
            .RoundBidding(
                gameProviderId: Arg.Is(command.Record.GameProviderId),
                gameProviderCode: Arg.Is(command.Record.GameProviderCode),
                externalRoundId: Arg.Is(command.Record.ExternalRoundId),
                gameId: Arg.Is(command.Record.GameId),
                externalGameId: Arg.Is(command.Record.ExternalGameId),
                playerCode: Arg.Is(command.Record.PlayerCode),
                walletId: Arg.Is(command.Record.WalletId),
                currencyCode: Arg.Is(command.Record.CurrencyCode),
                brandId: Arg.Is(command.Record.BrandId),
                gameProviderConfigurationId: Arg.Is(command.Record.GameProviderConfigurationId),
                status: Arg.Is(RoundStatus.Closed),
                clientStatus: Arg.Is(RoundClientStatus.Loss),
                externalCreatedOn: Arg.Is(command.Record.ExternalCreatedOn),
                externalClosedOn: Arg.Is(command.Record.ExternalClosedOn),
                bet: Arg.Is(command.Record.Bet),
                validBet: Arg.Is(command.Record.ValidBet),
                settle: Arg.Is(command.Record.Settle),
                commission: Arg.Is(command.Record.Commission),
                turnover: Arg.Is(command.Record.Turnover),
                playerWinLoss: Arg.Is(command.Record.WinLoss),
                additionalData: Arg.Is(command.Record.AdditionalData),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }

    [Fact]
    public async void Normal_Process_Settlement_Win()
    {
        var fakeClient = Substitute.For<IGameRoundsWorkClient>();

        var command = new HandleRecordCommand(
            Record: new RecordInfo(
                Success: true,
                GameProviderId: Guid.NewGuid(),
                GameProviderCode: "JILITF",
                ExternalRoundId: "1680840597057143115",
                GameId: 1,
                ExternalGameId: "115",
                PlayerCode: "4uiuj5ovw7zk",
                WalletId: Guid.NewGuid(),
                CurrencyCode: "CNY",
                BrandId: Guid.NewGuid(),
                GameProviderConfigurationId: Guid.NewGuid(),
                Status: (GamePullRecordStatus)1,
                ExternalCreatedOn: DateTimeOffset.Parse("2023-04-17T21:02:01-04:00"),
                ExternalClosedOn: DateTimeOffset.Parse("2023-04-17T21:02:01-04:00"),
                Bet: 1,
                ValidBet: 1,
                Settle: 3.4m,
                Commission: 0,
                Turnover: 1,
                WinLoss: 2.4m,
                AdditionalData: "{\"Account\":\"4uiuj5ovw7zk\",\"WagersId\":1680840597057143115,\"GameId\":115,\"WagersTime\":\"2023-04-17T21:02:01-04:00\",\"BetAmount\":-1,\"PayoffTime\":\"2023-04-17T21:02:01-04:00\",\"PayoffAmount\":3.4,\"Status\":1,\"SettlementTime\":\"2023-04-17T21:02:01-04:00\",\"GameCategoryId\":1,\"VersionKey\":0,\"Type\":1,\"AgentId\":\"PG_GPI_CNY\",\"Turnover\":1}"
                ));

        var service = new HandleRecordCommandService(fakeClient);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        await fakeClient
            .Received(1)
            .RoundBidding(
                gameProviderId: Arg.Is(command.Record.GameProviderId),
                gameProviderCode: Arg.Is(command.Record.GameProviderCode),
                externalRoundId: Arg.Is(command.Record.ExternalRoundId),
                gameId: Arg.Is(command.Record.GameId),
                externalGameId: Arg.Is(command.Record.ExternalGameId),
                playerCode: Arg.Is(command.Record.PlayerCode),
                walletId: Arg.Is(command.Record.WalletId),
                currencyCode: Arg.Is(command.Record.CurrencyCode),
                brandId: Arg.Is(command.Record.BrandId),
                gameProviderConfigurationId: Arg.Is(command.Record.GameProviderConfigurationId),
                status: Arg.Is(RoundStatus.Closed),
                clientStatus: Arg.Is(RoundClientStatus.Win),
                externalCreatedOn: Arg.Is(command.Record.ExternalCreatedOn),
                externalClosedOn: Arg.Is(command.Record.ExternalClosedOn),
                bet: Arg.Is(command.Record.Bet),
                validBet: Arg.Is(command.Record.ValidBet),
                settle: Arg.Is(command.Record.Settle),
                commission: Arg.Is(command.Record.Commission),
                turnover: Arg.Is(command.Record.Turnover),
                playerWinLoss: Arg.Is(command.Record.WinLoss),
                additionalData: Arg.Is(command.Record.AdditionalData),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }

    [Fact]
    public async void Normal_Process_Success_Is_False()
    {
        var fakeClient = Substitute.For<IGameRoundsWorkClient>();

        var command = new HandleRecordCommand(
            Record: new RecordInfo(
                Success: false,
                GameProviderId: Guid.NewGuid(),
                GameProviderCode: "JILITF",
                ExternalRoundId: "1680840597057153115",
                GameId: 1,
                ExternalGameId: "115",
                PlayerCode: "4uiuj5ovw7zk",
                WalletId: Guid.NewGuid(),
                CurrencyCode: "CNY",
                BrandId: Guid.NewGuid(),
                GameProviderConfigurationId: Guid.NewGuid(),
                Status: (GamePullRecordStatus)2,
                ExternalCreatedOn: DateTimeOffset.Parse("2023-04-10 23:45:22+08:00"),
                ExternalClosedOn: null,
                Bet: 1m,
                ValidBet: 1m,
                Settle: 0,
                Commission: 0,
                Turnover: 1,
                WinLoss: -1m,
                AdditionalData: "{\"Account\":\"4uiuj5ovw7zk\",\"WagersId\":1680840597057153115,\"GameId\":115,\"WagersTime\":\"2023-04-17T21:02:09-04:00\",\"BetAmount\":-1,\"PayoffTime\":\"2023-04-17T21:02:09-04:00\",\"PayoffAmount\":0,\"Status\":2,\"SettlementTime\":\"2023-04-17T21:02:09-04:00\",\"GameCategoryId\":1,\"VersionKey\":0,\"Type\":1,\"AgentId\":\"PG_GPI_CNY\",\"Turnover\":1}"
                ));

        var service = new HandleRecordCommandService(fakeClient);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        await fakeClient
            .Received(0)
            .RoundBidding(
                gameProviderId: Arg.Is(command.Record.GameProviderId),
                gameProviderCode: Arg.Is(command.Record.GameProviderCode),
                externalRoundId: Arg.Is(command.Record.ExternalRoundId),
                gameId: Arg.Is(command.Record.GameId),
                externalGameId: Arg.Is(command.Record.ExternalGameId),
                playerCode: Arg.Is(command.Record.PlayerCode),
                walletId: Arg.Is(command.Record.WalletId),
                currencyCode: Arg.Is(command.Record.CurrencyCode),
                brandId: Arg.Is(command.Record.BrandId),
                gameProviderConfigurationId: Arg.Is(command.Record.GameProviderConfigurationId),
                status: Arg.Is(RoundStatus.Open),
                clientStatus: Arg.Is(RoundClientStatus.Open),
                externalCreatedOn: Arg.Is(command.Record.ExternalCreatedOn),
                externalClosedOn: Arg.Is(command.Record.ExternalClosedOn),
                bet: Arg.Is(command.Record.Bet),
                validBet: Arg.Is(command.Record.ValidBet),
                settle: Arg.Is(command.Record.Settle),
                commission: Arg.Is(command.Record.Commission),
                turnover: Arg.Is(command.Record.Turnover),
                playerWinLoss: Arg.Is(command.Record.WinLoss),
                additionalData: Arg.Is(command.Record.AdditionalData),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);

        await fakeClient
            .Received(1)
            .RoundMissing(
                gameProviderId: Arg.Is(command.Record.GameProviderId),
                gameProviderCode: Arg.Is(command.Record.GameProviderCode),
                externalRoundId: Arg.Is(command.Record.ExternalRoundId),
                gameId: Arg.Is(command.Record.GameId),
                externalGameId: Arg.Is(command.Record.ExternalGameId),
                playerCode: Arg.Is(command.Record.PlayerCode),
                data: Arg.Is(JsonSerializer.Serialize(command.Record)),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }
}
