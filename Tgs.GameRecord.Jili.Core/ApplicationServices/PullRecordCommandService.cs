using Microsoft.Extensions.Logging;
using Tgs.GameInterface.Jili.Client;
using Tgs.GameInterface.JiliService.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Jili.Core.ApplicationServices;

internal class PullRecordCommandService(
    ILogger<PullRecordCommandService> logger,
    IGameInterfaceJiliClient client,
    ICommandService<HandleRecordCommand> service) : ICommandService<PullRecordCommand>
{
    public async ValueTask ExecuteAsync(PullRecordCommand command, CancellationToken cancellationToken = default)
    {
        var reply = await client
            .GetGamePullRecordAsync(
            apiUrl: command.ApiUrl,
            agentId: command.AgentId,
            agentKey: command.AgentKey,
            startTime: command.StartTime,
            endTime: command.EndTime,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameRecords = reply.Items ?? Enumerable.Empty<GamePullRecord>();

        logger.LogDebug("LogOn:{logOn} Message: Settlement. GameRecords:{gameRecords}", DateTime.Now.ToString("o"), gameRecords);

        if (!gameRecords.Any())
        {
            return;
        }

        foreach (var gameRecord in gameRecords)
        {
            logger.LogDebug("LogOn:{logOn} Message: Settlement. GameRecords:{gameRecord}", DateTime.Now.ToString("o"), gameRecord);

            try
            {
                await service
                    .ExecuteAsync(
                    command: new(gameRecord),
                    cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "LogOn:{logOn} Message: An exception occurred in this game record.({gameRecord})", DateTime.Now.ToString("o"), gameRecord);
            }
        }
    }
}
