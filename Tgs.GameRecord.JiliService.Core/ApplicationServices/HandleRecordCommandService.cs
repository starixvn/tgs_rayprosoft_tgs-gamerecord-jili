using Microsoft.Extensions.Options;
using Tgs.GameInterface.JiliService.Protos;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.JiliService.Core.ApplicationServices;

internal class HandleRecordCommandService : ICommandService<HandleRecordCommand>
{
    private readonly IGameRecordHandleClient _client;
    private readonly GameRecordInfoOptions _gameRecordInfo;

    public HandleRecordCommandService(
        IOptions<GameRecordInfoOptions> options,
        IGameRecordHandleClient client)
    {
        _gameRecordInfo = options.Value;
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
    {
        var status = GameRecordStatus.Closed;
        var externalCreatedOn = command.Record.WagersTime.ToDateTimeOffset();
        var externalClosedOn = command.Record.PayoffTime.ToDateTimeOffset();
        var bet = command.Record.BetAmount.ToDecimal().GetValueOrDefault(0);
        var validBet = command.Record.Turnover.ToDecimal().GetValueOrDefault(0);
        var settle = command.Record.PayoffAmount.ToDecimal().GetValueOrDefault(0);
        var commission = 0.0m;
        var turnover = command.Record.Turnover.ToDecimal().GetValueOrDefault(0);
        var playerWinLoss = settle - bet;
        var clientStatus = command.Record.Status == GamePullRecordStatus.JiliStatusWin ? GameRecordClientStatus.Win : GameRecordClientStatus.Loss;

        await _client
            .MessageAsync(
            gameProviderCode: _gameRecordInfo.GameProviderCode,
            externalRoundId: command.Record.WagersId,
            externalGameId: command.Record.GameId.ToString(),
            playerCode: command.Record.Account,
            status: status,
            clientStatus: clientStatus,
            externalCreatedOn: externalCreatedOn,
            externalClosedOn: externalClosedOn,
            bet: bet,
            validBet: validBet,
            settle: settle,
            commission: commission,
            turnover: turnover,
            playerWinLoss: playerWinLoss,
            additionalData: command.Record.AdditionalData,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
