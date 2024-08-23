using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameInterface.Jili.Protos;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Jili.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Jili.Core.UnitTest.ApplicationServices;

public class HandleRecordCommandServiceUnitTest
{
	[Fact]
	public async Task Loss()
	{
		var faceLogger = NullLogger<HandleRecordCommandService>.Instance;
		var fakeGameRecordInfoOptions = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "JILITF",
			GameProviderConfiguration = "JILITFConfiguration",
			HangfirePrefix = "gamerecord-jili-hangfire:",
			HangfireLocking = "gamerecord-jili-locking:",
			IntervalMinutes = 5,
			IntervalMinutesMax = 10,
			IntervalMinutesMinimum = 1
		});
		var fakePolicySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			RetryCount = 1,
			RetrySeconds = 1,
		});
		var fakeGameRecordHandleClient = Substitute.For<IGameRecordHandleClient>();

		var command = new HandleRecordCommand(
			Record: new GameRecordInfo
			{
				Account = "4uk946xvbpcd",
				WagersId = "1692153486628666049",
				GameId = 49,
				WagersTime = DateTimeOffset.Parse("2023-08-15T22:48:59-04:00").ToTimestamp(),
				BetAmount = 1m,
				PayoffTime = DateTimeOffset.Parse("2023-08-15T22:48:59-04:00").ToTimestamp(),
				PayoffAmount = 0.8m,
				Status = (GameRecordStatus)2,
				SettlementTime = DateTimeOffset.Parse("2023-08-15T22:48:59-04:00").ToTimestamp(),
				GameCategoryId = 1,
				Type = 1,
				AgentId = "PG_PB2_PHP",
				Turnover = 1m,
				AdditionalData = "{\"Account\":\"4uk946xvbpcd\",\"WagersId\":1692153486628666049,\"GameId\":49,\"WagersTime\":\"2023-08-15T22:48:59-04:00\",\"BetAmount\":-1,\"PayoffTime\":\"2023-08-15T22:48:59-04:00\",\"PayoffAmount\":0.8,\"Status\":2,\"SettlementTime\":\"2023-08-15T22:48:59-04:00\",\"GameCategoryId\":1,\"VersionKey\":0,\"Type\":1,\"AgentId\":\"PG_PB2_PHP\",\"Turnover\":1}"
			});

		var sut = new HandleRecordCommandService(
			faceLogger,
			fakeGameRecordInfoOptions,
			fakePolicySettingsOptions,
			fakeGameRecordHandleClient);

		await sut.ExecuteAsync(command);

		_ = fakeGameRecordHandleClient
			.Received(1)
			.HandleAsync(
			source: Arg.Is(new Handle.Common.ViewModels.GameRecordHandleViewModel
			{
				GameProviderCode = fakeGameRecordInfoOptions.Value.GameProviderCode,
				PlayerCode = command.Record.Account,
				ExternalRoundId = command.Record.WagersId,
				ExternalBetId = command.Record.WagersId,
				ExternalGameId = command.Record.GameId.ToString(),
				ExternalCreatedOn = command.Record.WagersTime.ToDateTimeOffset(),
				ExternalClosedOn = command.Record.PayoffTime.ToDateTimeOffset(),
				ExternalPayoffOn = command.Record.PayoffTime.ToDateTimeOffset(),
				ExternalResettleOn = DateTimeOffset.UnixEpoch,
				BetState = Handle.Common.Protos.BetState.Loss,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 1m,
				ValidBet = 1m,
				Settle = 0.8m,
				Commission = 0.0M,
				Turnover = 1m,
				WinLoss = -0.2m,
				Jackpot = 0.0M,
				AdditionalData = command.Record.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Win()
	{
		var faceLogger = NullLogger<HandleRecordCommandService>.Instance;
		var fakeGameRecordInfoOptions = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "JILITF",
			GameProviderConfiguration = "JILITFConfiguration",
			HangfirePrefix = "gamerecord-jili-hangfire:",
			HangfireLocking = "gamerecord-jili-locking:",
			IntervalMinutes = 5,
			IntervalMinutesMax = 10,
			IntervalMinutesMinimum = 1
		});
		var fakePolicySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			RetryCount = 1,
			RetrySeconds = 1,
		});
		var fakeGameRecordHandleClient = Substitute.For<IGameRecordHandleClient>();

		var command = new HandleRecordCommand(
			Record: new GameRecordInfo
			{
				Account = "4ukpmyqhh2d7",
				WagersId = "1692154079166114134",
				GameId = 134,
				WagersTime = DateTimeOffset.Parse("2023-08-15T22:48:59-04:00").ToTimestamp(),
				BetAmount = 5m,
				PayoffTime = DateTimeOffset.Parse("2023-08-15T22:48:59-04:00").ToTimestamp(),
				PayoffAmount = 27.7m,
				Status = (GameRecordStatus)1,
				SettlementTime = DateTimeOffset.Parse("2023-08-15T22:48:59-04:00").ToTimestamp(),
				GameCategoryId = 1,
				Type = 1,
				AgentId = "PG_PB2_PHP",
				Turnover = 5m,
				AdditionalData = "{\"Account\":\"4ukpmyqhh2d7\",\"WagersId\":1692154079166114134,\"GameId\":134,\"WagersTime\":\"2023-08-15T22:48:59-04:00\",\"BetAmount\":-5,\"PayoffTime\":\"2023-08-15T22:48:59-04:00\",\"PayoffAmount\":27.7,\"Status\":1,\"SettlementTime\":\"2023-08-15T22:48:59-04:00\",\"GameCategoryId\":1,\"VersionKey\":0,\"Type\":1,\"AgentId\":\"PG_PB2_PHP\",\"Turnover\":5}"
			});

		var sut = new HandleRecordCommandService(
			faceLogger,
			fakeGameRecordInfoOptions,
			fakePolicySettingsOptions,
			fakeGameRecordHandleClient);

		await sut.ExecuteAsync(command);

		_ = fakeGameRecordHandleClient
			.Received(1)
			.HandleAsync(
			source: Arg.Is(new Handle.Common.ViewModels.GameRecordHandleViewModel
			{
				GameProviderCode = fakeGameRecordInfoOptions.Value.GameProviderCode,
				PlayerCode = command.Record.Account,
				ExternalRoundId = command.Record.WagersId,
				ExternalBetId = command.Record.WagersId,
				ExternalGameId = command.Record.GameId.ToString(),
				ExternalCreatedOn = command.Record.WagersTime.ToDateTimeOffset(),
				ExternalClosedOn = command.Record.PayoffTime.ToDateTimeOffset(),
				ExternalPayoffOn = command.Record.PayoffTime.ToDateTimeOffset(),
				ExternalResettleOn = DateTimeOffset.UnixEpoch,
				BetState = Handle.Common.Protos.BetState.Win,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 5.0m,
				ValidBet = 5.0m,
				Settle = 27.7m,
				Commission = 0.0M,
				Turnover = 5.0m,
				WinLoss = 22.7m,
				Jackpot = 0.0M,
				AdditionalData = command.Record.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
