using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameInterface.JiliService.Protos;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.GameRecord.JiliService.Core.ApplicationServices;

namespace Tgs.GameRecord.JiliService.Core.UnitTest.ApplicationServices;

public class HandleRecordCommandServiceUnitTest
{
    [Fact]
    public async void Loss()
    {
        var gameRecordInfo = Options.Create(new GameRecordInfoOptions
        {
            GameProviderCode = "JILITF",
            PageLimit = 10000
        });
        var fakeClient = Substitute.For<IGameRecordHandleClient>();

        var command = new HandleRecordCommand(
            Record: new GamePullRecord
            {
                Account = "4uk946xvbpcd",
                WagersId = "1692153486628666049",
                GameId = 49,
                WagersTime = DateTimeOffset.Parse("2023-08-15T22:48:59-04:00").ToTimestamp(),
                BetAmount = 1m,
                PayoffTime = DateTimeOffset.Parse("2023-08-15T22:48:59-04:00").ToTimestamp(),
                PayoffAmount = 0.8m,
                Status = (GamePullRecordStatus)2,
                SettlementTime = DateTimeOffset.Parse("2023-08-15T22:48:59-04:00").ToTimestamp(),
                GameCategoryId = 1,
                Type = 1,
                AgentId = "PG_PB2_PHP",
                Turnover = 1m,
                AdditionalData = "{\"Account\":\"4uk946xvbpcd\",\"WagersId\":1692153486628666049,\"GameId\":49,\"WagersTime\":\"2023-08-15T22:48:59-04:00\",\"BetAmount\":-1,\"PayoffTime\":\"2023-08-15T22:48:59-04:00\",\"PayoffAmount\":0.8,\"Status\":2,\"SettlementTime\":\"2023-08-15T22:48:59-04:00\",\"GameCategoryId\":1,\"VersionKey\":0,\"Type\":1,\"AgentId\":\"PG_PB2_PHP\",\"Turnover\":1}"
            });

        var service = new HandleRecordCommandService(
            gameRecordInfo,
            fakeClient);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        await fakeClient
            .Received(1)
            .MessageAsync(
            gameProviderCode: Arg.Is(gameRecordInfo.Value.GameProviderCode),
            externalRoundId: Arg.Is(command.Record.WagersId),
            externalGameId: Arg.Is(command.Record.GameId.ToString()),
            playerCode: Arg.Is(command.Record.Account),
            status: Arg.Is(GameRecordStatus.Closed),
            clientStatus: Arg.Is(GameRecordClientStatus.Loss),
            externalCreatedOn: Arg.Is(command.Record.WagersTime.ToDateTimeOffset()),
            externalClosedOn: Arg.Is(command.Record.PayoffTime.ToDateTimeOffset()),
            bet: Arg.Is(1m),
            validBet: Arg.Is(1m),
            settle: Arg.Is(0.8m),
            commission: Arg.Is(0.0m),
            turnover: Arg.Is(1m),
            playerWinLoss: Arg.Is(-0.2m),
            additionalData: Arg.Is(command.Record.AdditionalData),
            cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }

    [Fact]
    public async void Win()
    {
        var gameRecordInfo = Options.Create(new GameRecordInfoOptions
        {
            GameProviderCode = "JILITF",
            PageLimit = 10000
        });
        var fakeClient = Substitute.For<IGameRecordHandleClient>();

        var command = new HandleRecordCommand(
            Record: new GamePullRecord
            {
                Account = "4ukpmyqhh2d7",
                WagersId = "1692154079166114134",
                GameId = 134,
                WagersTime = DateTimeOffset.Parse("2023-08-15T22:48:59-04:00").ToTimestamp(),
                BetAmount = 5m,
                PayoffTime = DateTimeOffset.Parse("2023-08-15T22:48:59-04:00").ToTimestamp(),
                PayoffAmount = 27.7m,
                Status = (GamePullRecordStatus)1,
                SettlementTime = DateTimeOffset.Parse("2023-08-15T22:48:59-04:00").ToTimestamp(),
                GameCategoryId = 1,
                Type = 1,
                AgentId = "PG_PB2_PHP",
                Turnover = 5m,
                AdditionalData = "{\"Account\":\"4ukpmyqhh2d7\",\"WagersId\":1692154079166114134,\"GameId\":134,\"WagersTime\":\"2023-08-15T22:48:59-04:00\",\"BetAmount\":-5,\"PayoffTime\":\"2023-08-15T22:48:59-04:00\",\"PayoffAmount\":27.7,\"Status\":1,\"SettlementTime\":\"2023-08-15T22:48:59-04:00\",\"GameCategoryId\":1,\"VersionKey\":0,\"Type\":1,\"AgentId\":\"PG_PB2_PHP\",\"Turnover\":5}"
            });

        var service = new HandleRecordCommandService(
            gameRecordInfo,
            fakeClient);

        await service
            .ExecuteAsync(
            command)
            .ConfigureAwait(false);

        await fakeClient
            .Received(1)
            .MessageAsync(
            gameProviderCode: Arg.Is(gameRecordInfo.Value.GameProviderCode),
            externalRoundId: Arg.Is(command.Record.WagersId),
            externalGameId: Arg.Is(command.Record.GameId.ToString()),
            playerCode: Arg.Is(command.Record.Account),
            status: Arg.Is(GameRecordStatus.Closed),
            clientStatus: Arg.Is(GameRecordClientStatus.Win),
            externalCreatedOn: Arg.Is(command.Record.WagersTime.ToDateTimeOffset()),
            externalClosedOn: Arg.Is(command.Record.PayoffTime.ToDateTimeOffset()),
            bet: Arg.Is(5m),
            validBet: Arg.Is(5m),
            settle: Arg.Is(27.7m),
            commission: Arg.Is(0.0m),
            turnover: Arg.Is(5m),
            playerWinLoss: Arg.Is(22.7m),
            additionalData: Arg.Is(command.Record.AdditionalData),
            cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }
}
