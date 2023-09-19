using Microsoft.Extensions.Options;
using Tgs.GameInterface.JiliService.Protos;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Jili.Core.ApplicationServices;

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

    public ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
    {
        var wagerState = command.Record.Status == GamePullRecordStatus.JiliStatusWin ? WagerStates.Win : WagerStates.Lose;
        var externalCreatedOn = command.Record.WagersTime.ToDateTimeOffset();
        var externalClosedOn = command.Record.PayoffTime.ToDateTimeOffset();
        var bet = command.Record.BetAmount.ToDecimal().GetValueOrDefault(0);
        var turnover = command.Record.Turnover.ToDecimal().GetValueOrDefault(0);
        var settle = command.Record.PayoffAmount.ToDecimal().GetValueOrDefault(0);
        var commission = 0.0m;
        var winLoss = settle - bet;

        return _client
            .MessageAsync(
            gameProviderCode: _gameRecordInfo.GameProviderCode,
            playerCode: command.Record.Account,
            wagerState: wagerState,
            wagerType: WagerTypes.Normal,
            freeGameState: FreeGameStates.None,
            gambleState: GambleStates.None,
            cashoutState: CashoutStates.None,
            resettleState: ResettleStates.None,
            externalRoundId: command.Record.WagersId,
            externalWagerId: command.Record.WagersId,
            externalGameId: command.Record.GameId.ToString(),
            externalCreatedOn: externalCreatedOn,
            externalClosedOn: externalClosedOn,
            externalPayoffOn: externalClosedOn,
            externalResettleOn: DateTimeOffset.UnixEpoch,
            bet: bet,
            validBet: turnover,
            settle: settle,
            commission: commission,
            turnover: turnover,
            winLoss: winLoss,
            additionalData: command.Record.AdditionalData,
            cancellationToken: cancellationToken);
    }
}
