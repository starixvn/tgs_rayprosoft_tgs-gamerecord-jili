using System.Text.Json.Nodes;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameInterface.JiliService.Protos;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.GameRecord.Jili.Core.ApplicationServices;

namespace Tgs.GameRecord.Jili.Core.UnitTest.ApplicationServices;

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

		await service.ExecuteAsync(command);

		await fakeClient
			.Received(1)
			.MessageAsync(
			gameProviderCode: Arg.Is(gameRecordInfo.Value.GameProviderCode),
			playerCode: Arg.Is(command.Record.Account),
			wagerState: Arg.Is(WagerStates.Lose),
			wagerType: Arg.Is(WagerTypes.Normal),
			freeGameState: Arg.Is(FreeGameStates.None),
			gambleState: Arg.Is(GambleStates.None),
			cashoutState: Arg.Is(CashoutStates.None),
			resettleState: Arg.Is(ResettleStates.None),
			externalRoundId: Arg.Is(command.Record.WagersId),
			externalWagerId: Arg.Is(command.Record.WagersId),
			externalGameId: Arg.Is(command.Record.GameId.ToString()),
			externalCreatedOn: Arg.Is(command.Record.WagersTime.ToDateTimeOffset()),
			externalClosedOn: Arg.Is(command.Record.PayoffTime.ToDateTimeOffset()),
			externalPayoffOn: Arg.Is(command.Record.PayoffTime.ToDateTimeOffset()),
			externalResettleOn: Arg.Is(DateTimeOffset.UnixEpoch),
			bet: Arg.Is(1m),
			validBet: Arg.Is(1m),
			settle: Arg.Is(0.8m),
			commission: Arg.Is(0.0m),
			turnover: Arg.Is(1m),
			winLoss: Arg.Is(-0.2m),
			additionalData: Arg.Any<JsonNode>(),
			cancellationToken: Arg.Any<CancellationToken>());
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

		await service.ExecuteAsync(command);

		await fakeClient
			.Received(1)
			.MessageAsync(
			gameProviderCode: Arg.Is(gameRecordInfo.Value.GameProviderCode),
			playerCode: Arg.Is(command.Record.Account),
			wagerState: Arg.Is(WagerStates.Win),
			wagerType: Arg.Is(WagerTypes.Normal),
			freeGameState: Arg.Is(FreeGameStates.None),
			gambleState: Arg.Is(GambleStates.None),
			cashoutState: Arg.Is(CashoutStates.None),
			resettleState: Arg.Is(ResettleStates.None),
			externalRoundId: Arg.Is(command.Record.WagersId),
			externalWagerId: Arg.Is(command.Record.WagersId),
			externalGameId: Arg.Is(command.Record.GameId.ToString()),
			externalCreatedOn: Arg.Is(command.Record.WagersTime.ToDateTimeOffset()),
			externalClosedOn: Arg.Is(command.Record.PayoffTime.ToDateTimeOffset()),
			externalPayoffOn: Arg.Is(command.Record.PayoffTime.ToDateTimeOffset()),
			externalResettleOn: Arg.Is(DateTimeOffset.UnixEpoch),
			bet: Arg.Is(5m),
			validBet: Arg.Is(5m),
			settle: Arg.Is(27.7m),
			commission: Arg.Is(0.0m),
			turnover: Arg.Is(5m),
			winLoss: Arg.Is(22.7m),
			additionalData: Arg.Any<JsonNode>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
