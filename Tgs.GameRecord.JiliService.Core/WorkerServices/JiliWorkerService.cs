using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameInterface.JiliService.Client;
using Tgs.GameRecord.JiliService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.JiliService.Core.WorkerServices;

internal class JiliWorkerService : ICommandService<JiliWorkerCommand>
{
    private readonly GameRecordInfoOptions _gameRecordInfo;
    private readonly IGameInterfaceJiliServiceGrpc _grpc;
    private readonly ILogger<JiliWorkerService> _logger;
    private readonly ICommandService<GamePullRecordCommand> _service;
    private readonly IUtensil _utensil;

    public JiliWorkerService(
        IOptions<GameRecordInfoOptions> options,
        IGameInterfaceJiliServiceGrpc grpc,
        ILogger<JiliWorkerService> logger,
        IUtensil utensil,
        ICommandService<GamePullRecordCommand> service)
    {
        _gameRecordInfo = options.Value;
        _grpc = grpc ?? throw new ArgumentNullException(nameof(grpc));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask ExecuteAsync(JiliWorkerCommand command, CancellationToken cancellationToken = default)
    {
        int page = 1;
        int pageLimit = _gameRecordInfo.PageLimit;

        var gameProviderId = await _utensil.GetGameProviderIdAsync(_gameRecordInfo.GameProviderCode).ConfigureAwait(false);

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

            await _service.ExecuteAsync(new GamePullRecordCommand(
                GameProviderId: gameProviderId,
                GameProviderCode: _gameRecordInfo.GameProviderCode,
                Records: gamePullRecordReply.Items), cancellationToken: cancellationToken).ConfigureAwait(false);

            page++;
        }
    }
}
