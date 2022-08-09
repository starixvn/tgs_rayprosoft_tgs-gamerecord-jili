using System.Text.Json;
using Tgs.GameInterface.JiliService.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.JiliService.Core.ApplicationServices
{
    internal class GamePullRecordBatchCommandService : ICommandService<GamePullRecordBatchCommand>
    {
        private readonly ICommandService<GameRoundBatchCommand> _service;
        private readonly IUtensil _utensil;

        public GamePullRecordBatchCommandService(
            ICommandService<GameRoundBatchCommand> service,
            IUtensil utensil)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
        }

        public async ValueTask ExecuteAsync(GamePullRecordBatchCommand command, CancellationToken cancellationToken = default)
        {
            var gameRecords = new List<GameRecord>();

            foreach (var record in command.Records)
            {
                bool success = true;
                var playerModel = await _utensil.GetPlayerAsync(record.Account).ConfigureAwait(false);
                var gameId = await _utensil.GetGameIdAsync(command.GameProviderId, record.GameId.ToString()).ConfigureAwait(false);
                if (playerModel == null || !gameId.HasValue)
                {
                    success = false;
                }

                Guid? gameProviderConfigurationId = default;
                if (playerModel != null)
                {
                    gameProviderConfigurationId = await _utensil.BrandGetGameProviderConfigurationIdAsync(playerModel.BrandId, command.GameProviderId).ConfigureAwait(false);
                    if (!gameProviderConfigurationId.HasValue)
                    {
                        success = false;
                    }
                }
                var status = GameRoundStatus.Closed;
                var clientStatus = record.Status == GameInterfaceJiliServiceGamePullRecordStatus.JiliStatusWin ? GameRoundClientStatus.Win : GameRoundClientStatus.Loss;
                var playerWinLoss = record.PayoffAmount - record.BetAmount;

                var additionalData = new
                {
                    record.Account,
                    record.WagersId,
                    record.GameId,
                    record.WagersTime,
                    BetAmount = record.BetAmount.ToDecimal(),
                    record.PayoffTime,
                    PayoffAmount = record.PayoffAmount.ToDecimal(),
                    Status = record.Status.ToString(),
                    record.SettlementTime,
                    record.GameCategoryId,
                    record.Type,
                    record.AgentId
                };

                gameRecords.Add(new GameRecord(
                    ExternalRoundId: record.WagersId,
                    GameProviderId: command.GameProviderId,
                    GameProviderCode: command.GameProviderCode,
                    PlayerCode: (playerModel == null) ? string.Empty : playerModel.PlayerCode,
                    GameId: gameId ?? -1,
                    ExternalGameId: record.GameId.ToString(),
                    CurrencyCode: (playerModel == null) ? string.Empty : playerModel.DefaultWalletCurrencyCode,
                    Status: status,
                    GameProviderConfigurationId: gameProviderConfigurationId ?? Guid.Empty,
                    WalletId: (playerModel == null) ? Guid.Empty : playerModel.DefaultWalletId,
                    ExternalCreatedOn: record.WagersTime.ToDateTimeOffset(),
                    ExternalClosedOn: record.PayoffTime.ToDateTimeOffset(),
                    BrandId: (playerModel == null) ? Guid.Empty : playerModel.BrandId,
                    BetAmount: record.BetAmount,
                    ValidBetAmount: record.BetAmount,
                    SettleAmount: record.PayoffAmount,
                    Commission: 0,
                    Turnover: record.BetAmount,
                    PlayerWinLoss: playerWinLoss,
                    AdditionalData: JsonSerializer.Serialize(additionalData),
                    ClientStatus: clientStatus,
                    Success: success));
            }

            await _service.ExecuteAsync(new GameRoundBatchCommand(GameRecords: gameRecords)).ConfigureAwait(false);
        }
    }
}
