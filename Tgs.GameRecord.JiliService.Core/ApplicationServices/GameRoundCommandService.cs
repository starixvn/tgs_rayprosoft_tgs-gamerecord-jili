using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.JiliService.Core.ApplicationServices
{
    internal class GameRoundCommandService : ICommandService<GameRoundCommand>
    {
        private readonly ILogger<GameRoundCommandService> _logger;
        private readonly IMessageSender _messageSender;

        public GameRoundCommandService(IMessageSender messageSender,
            ILogger<GameRoundCommandService> logger)
        {
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ValueTask ExecuteAsync(GameRoundCommand command, CancellationToken cancellationToken = default)
        {

            var subject = $"tgs.gp.gamerecord.database.round.{command.GameProviderCode}".ToLower();

            _logger.LogInformation("LogOn:{logOn}  Subject:{subject} MessageRequest:{request}", DateTime.Now.ToString("o"), subject, command);

            return _messageSender.PublishAsync(subject, command.ToBson(), cancellationToken);
        }
    }
}
