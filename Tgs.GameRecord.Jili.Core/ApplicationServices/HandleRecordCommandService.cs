using System.Text.Json.Nodes;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameInterface.JiliService.Protos;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Jili.Core.ApplicationServices;

internal class HandleRecordCommandService(
	ILogger<HandleRecordCommandService> logger,
	IOptions<GameRecordInfoOptions> options,
	IMessageSender messageSender) : ICommandService<HandleRecordCommand>
{
	private readonly GameRecordInfoOptions _gameRecordInfo = options.Value;

	public async ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
	{
		var betState = command.Record.Status == GamePullRecordStatus.JiliStatusWin ? Handle.Common.Protos.BetState.Win : Handle.Common.Protos.BetState.Loss;
		var externalCreatedOn = command.Record.WagersTime.ToDateTimeOffset();
		var externalClosedOn = command.Record.PayoffTime.ToDateTimeOffset();
		var bet = command.Record.BetAmount.ToDecimal().GetValueOrDefault(0);
		var turnover = command.Record.Turnover.ToDecimal().GetValueOrDefault(0);
		var settle = command.Record.PayoffAmount.ToDecimal().GetValueOrDefault(0);
		var winLoss = settle - bet;

		var request = new Handle.Common.Protos.GameRecordHandleRequest
		{
			GameProviderCode = _gameRecordInfo.GameProviderCode,
			PlayerCode = command.Record.Account,
			ExternalRoundId = command.Record.WagersId,
			ExternalBetId = command.Record.WagersId,
			ExternalGameId = command.Record.GameId.ToString(),
			ExternalCreatedOn = externalCreatedOn.ToTimestamp(),
			ExternalClosedOn = externalClosedOn.ToTimestamp(),
			ExternalPayoffOn = externalClosedOn.ToTimestamp(),
			ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
			BetState = betState,
			BetType = Handle.Common.Protos.BetType.Normal,
			FreeGameState = Handle.Common.Protos.FreeGameState.None,
			GambleState = Handle.Common.Protos.GambleState.None,
			CashoutState = Handle.Common.Protos.CashoutState.None,
			ResettleState = Handle.Common.Protos.ResettleState.None,
			JackpotState = Handle.Common.Protos.JackpotState.None,
			Bet = bet,
			ValidBet = turnover,
			Settle = settle,
			Commission = 0.0m,
			Turnover = turnover,
			WinLoss = winLoss,
			Jackpot = 0.0M,
			AdditionalData = command.Record.AdditionalData
		};

		await messageSender.PublishAsync(
			subject: Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest,
			data: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This publication requests information.({request})",
			DateTime.Now.ToString("o"),
			Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest,
			new
			{
				request.GameProviderCode,
				request.PlayerCode,
				request.ExternalRoundId,
				request.ExternalBetId,
				request.ExternalGameId,
				ExternalCreatedOn = request.ExternalCreatedOn.ToDateTimeOffset(),
				ExternalClosedOn = request.ExternalClosedOn.ToDateTimeOffset(),
				request.BetState
			});
	}
}
