using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameRecord.JiliService.Core.WorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.JiliService.Core.DispatchServices;

internal class ScheduleDispatchService : ICommandService<ScheduleDispatchCommand>
{
    private readonly ICommandService<ScheduleWorkerCommand> _commandService;
    private readonly IDispatchLock _dispatchLock;
    private readonly GameRecordInfoOptions _gameRecordInfo;
    private readonly double _intervalMinute;
    private readonly ILogger<ScheduleDispatchService> _logger;

    public ScheduleDispatchService(
        ILogger<ScheduleDispatchService> logger,
        IOptions<GameRecordInfoOptions> options,
        IDispatchLock dispatchLock,
        ICommandService<ScheduleWorkerCommand> commandService)
    {
        _gameRecordInfo = options.Value;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
        _dispatchLock = dispatchLock ?? throw new ArgumentNullException(nameof(dispatchLock));
        _intervalMinute = (_gameRecordInfo.IntervalMinutes > _gameRecordInfo.IntervalMinutesMax)
                        ? _gameRecordInfo.IntervalMinutesMax
                        : (_gameRecordInfo.IntervalMinutes < _gameRecordInfo.IntervalMinutesMinimum)
                        ? _gameRecordInfo.IntervalMinutesMinimum
                        : _gameRecordInfo.IntervalMinutes;
    }

    public async ValueTask ExecuteAsync(ScheduleDispatchCommand command, CancellationToken cancellationToken = default)
    {
        var key = $"gamerecord:jili:dispatch:schedule:{command.GameProviderAgentId}";

        if (await _dispatchLock.LockingAsync(key: key, command: command).ConfigureAwait(false))
        {
            try
            {
                var endTime = DateTimeOffset.ParseExact(DateTimeOffset.UtcNow.ToString(@"yyyy-MM-dd\THH:mm:00K"), @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind);
                var startTime = endTime.AddMinutes(_intervalMinute * -1);

                var workerCommand = new ScheduleWorkerCommand(
                     GameApiUrl: command.GameApiUrl,
                     GameProviderAgentId: command.GameProviderAgentId,
                     GameProviderAgentKey: command.GameProviderAgentKey,
                     StartTime: startTime,
                     EndTime: endTime);

                _logger.LogInformation("LogOn:{logOn} WorkerCommand:{workerCommand}.",
                     DateTimeOffset.UtcNow.ToString("O"),
                     workerCommand);

                await _commandService
                    .ExecuteAsync(
                        command: workerCommand,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LogOn:{logOn} Message:An exception occurred on dispatch.(GameProviderAgentId:{gameProviderAgentId})",
                    DateTimeOffset.UtcNow.ToString("O"),
                    command.GameProviderAgentId);
            }
            finally
            {
                _ = await _dispatchLock.UnlockAsync(key: key).ConfigureAwait(false);
            }
        }
    }
}
