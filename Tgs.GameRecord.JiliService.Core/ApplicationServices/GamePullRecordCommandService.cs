using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Tgs.GameInterface.JiliService.Protos;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.MessageModels;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.JiliService.Core.ApplicationServices;

internal class GamePullRecordCommandService : ICommandService<GamePullRecordCommand>
{
    private readonly GameRecordInfoOptions _gameRecordInfo;
    private readonly ILogger<GamePullRecordCommandService> _logger;
    private readonly IMessageSender _messageSender;
    private readonly MongoSettingOptions _mongoSetting;
    private readonly IUtensil _utensil;

    public GamePullRecordCommandService(
        ILogger<GamePullRecordCommandService> logger,
        IOptions<GameRecordInfoOptions> options,
        IOptions<MongoSettingOptions> mongoSettingOptions,
        IUtensil utensil,
        IMessageSender messageSender)
    {
        _gameRecordInfo = options.Value;
        _mongoSetting = mongoSettingOptions.Value;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
        _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
    }

    public async ValueTask ExecuteAsync(GamePullRecordCommand command, CancellationToken cancellationToken = default)
    {
        foreach (var record in command.Records)
        {
            bool success = true;
            var playerModel = await _utensil
                .GetPlayerAsync(
                    playerCode: record.Account,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            var gameId = await _utensil
                .GetGameIdAsync(
                    gameProviderId: command.GameProviderId,
                    gameExternalId: record.GameId.ToString(),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            if (playerModel == null || !gameId.HasValue)
            {
                success = false;
            }

            Guid? gameProviderConfigurationId = default;
            if (playerModel != null)
            {
                gameProviderConfigurationId = await _utensil
                    .GetBrandGameProviderConfigurationIdAsync(
                        brandId: playerModel.BrandId,
                        gameProviderId: command.GameProviderId,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
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

            var message = new GameRecordMessage(
                ExternalRoundId: record.WagersId,
                GameProviderId: command.GameProviderId,
                GameProviderCode: _gameRecordInfo.GameProviderCode,
                GameId: gameId ?? -1,
                ExternalGameId: record.GameId.ToString(),
                PlayerCode: (playerModel == null) ? string.Empty : playerModel.PlayerCode,
                CurrencyCode: (playerModel == null) ? string.Empty : playerModel.DefaultWalletCurrencyCode,
                WalletId: (playerModel == null) ? Guid.Empty : playerModel.DefaultWalletId,
                BrandId: (playerModel == null) ? Guid.Empty : playerModel.BrandId,
                GameProviderConfigurationId: gameProviderConfigurationId ?? Guid.Empty,
                Status: status,
                ClientStatus: clientStatus,
                ExternalCreatedOn: record.WagersTime.ToDateTimeOffset(),
                ExternalClosedOn: record.PayoffTime.ToDateTimeOffset(),
                BetAmount: record.BetAmount.ToDecimal().GetValueOrDefault(0),
                ValidBetAmount: record.BetAmount.ToDecimal().GetValueOrDefault(0),
                SettleAmount: record.PayoffAmount.ToDecimal().GetValueOrDefault(0),
                Commission: 0,
                Turnover: record.BetAmount.ToDecimal().GetValueOrDefault(0),
                PlayerWinLoss: playerWinLoss,
                AdditionalData: JsonSerializer.Serialize(additionalData),
                Success: success);

            _logger.LogInformation("LogOn:{logOn}  Subject:{subject} MessageRequest:{message}",
                DateTime.Now.ToString("o"),
                _mongoSetting.GlobPattern,
                message);

            await _messageSender
                .PublishAsync(
                    subject: _mongoSetting.GlobPattern,
                    data: message.ToBson(),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
