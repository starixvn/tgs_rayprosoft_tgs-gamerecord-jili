using Microsoft.Extensions.Options;
using Tgs.GameRecord.JiliService.Core.WorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.JiliService.Core.DispatchServices
{
    internal class ScheduleDispatchService : ICommandService<ScheduleDispatchCommand>
    {
        private readonly ICommandService<JiliWorkerCommand> _commandService;
        private readonly GameRecordInfoOptions _gameRecordInfoOptions;
        private readonly IScheduleDispatch _scheduleDispatch;

        public ScheduleDispatchService(
            IOptions<GameRecordInfoOptions> options,
            ICommandService<JiliWorkerCommand> commandService,
            IScheduleDispatch scheduleDispatch
            )
        {
            _gameRecordInfoOptions = options.Value;
            _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            _scheduleDispatch = scheduleDispatch ?? throw new ArgumentNullException(nameof(scheduleDispatch));
        }

        public async ValueTask ExecuteAsync(ScheduleDispatchCommand command, CancellationToken cancellationToken = default)
        {
            var key = $"GameRecord:JiliService:{_gameRecordInfoOptions.GameProviderCode}:{command.GameProviderAgentId}".ToLower();
            if (!await _scheduleDispatch.ExistAsync(key: key).ConfigureAwait(false))
            {
                if (await _scheduleDispatch.LockingAsync(key: key, command: command).ConfigureAwait(false))
                {
                    try
                    {
                        var endTime = DateTimeOffset.ParseExact(DateTimeOffset.UtcNow.ToString(@"yyyy-MM-dd\THH:mm:00K"), @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind);
                        var startTime = endTime.AddMinutes((double)_gameRecordInfoOptions.IntervalMinute * -1);

                        await _commandService.ExecuteAsync(new JiliWorkerCommand(
                             GameApiUrl: command.GameApiUrl,
                             GameProviderAgentId: command.GameProviderAgentId,
                             GameProviderAgentKey: command.GameProviderAgentKey,
                             StartTime: startTime,
                             EndTime: endTime), cancellationToken: cancellationToken).ConfigureAwait(false);
                    }
                    finally
                    {
                        _ = await _scheduleDispatch.UnlockAsync(key: key).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}
