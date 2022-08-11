using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.JiliService.Core.ApplicationServices
{
    internal class GameRoundBatchCommandService : ICommandService<GameRoundBatchCommand>
    {
        private readonly ILogger<GameRoundBatchCommandService> _logger;
        private readonly IMessageSender _messageSender;
        private readonly GameRecordInfoOptions _gameRecordInfoOptions;


        public GameRoundBatchCommandService(
            IMessageSender messageSender,
            IOptions<GameRecordInfoOptions> options,
            ILogger<GameRoundBatchCommandService> logger)
        {
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            _gameRecordInfoOptions = options.Value;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ValueTask ExecuteAsync(GameRoundBatchCommand command, CancellationToken cancellationToken = default)
        {
            var subject = $"tgs.gp.gamerecord.database.round.batch.{_gameRecordInfoOptions.GameProviderCode}".ToLower();

            _logger.LogInformation("LogOn:{logOn}  Subject:{subject} MessageRequest:{request}", DateTime.Now.ToString("o"), subject, command);

            return _messageSender.PublishAsync(subject, command.ToBson(), cancellationToken);
        }
    }
}
