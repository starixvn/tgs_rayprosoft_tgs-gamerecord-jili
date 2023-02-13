using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameInterface.JiliService.Client;
using Tgs.GameRecord.JiliService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.JiliService.Core.WorkerServices;

internal class ScheduleWorkerService : ICommandService<ScheduleWorkerCommand>
{
    private readonly GameRecordInfoOptions _gameRecordInfo;
    private readonly IGameInterfaceJiliServiceGrpc _grpc;
    private readonly ILogger<ScheduleWorkerService> _logger;
    private readonly ICommandService<SettlementCommand> _service;

    public ScheduleWorkerService(
        IOptions<GameRecordInfoOptions> options,
        IGameInterfaceJiliServiceGrpc grpc,
        ILogger<ScheduleWorkerService> logger,
        ICommandService<SettlementCommand> service)
    {
        _gameRecordInfo = options.Value;
        _grpc = grpc ?? throw new ArgumentNullException(nameof(grpc));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask ExecuteAsync(ScheduleWorkerCommand command, CancellationToken cancellationToken = default)
    {
        int page = 1;
        int pageLimit = _gameRecordInfo.PageLimit;

        while (true)
        {
            var gamePullRecordReply = await _grpc.GetGamePullRecordAsync(
               apiUrl: command.GameApiUrl,
               agentId: command.GameProviderAgentId,
               agentKey: command.GameProviderAgentKey,
               startTime: command.StartTime,
               endTime: command.EndTime,
               page: page,
               pageLimit: pageLimit,
               cancellationToken: cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("LogOn:{logOn} | Request:{request} | Response:{response}",
                DateTime.UtcNow.ToString("O"),
                command,
                gamePullRecordReply);

            if (gamePullRecordReply == null)
            {
                break;
            }

            if (gamePullRecordReply.Items == null || !gamePullRecordReply.Items.Any())
            {
                break;
            }

            await _service.ExecuteAsync(
                command: new SettlementCommand(
                        Records: gamePullRecordReply.Items),
                cancellationToken: cancellationToken).ConfigureAwait(false);

            page++;
        }
    }
}
