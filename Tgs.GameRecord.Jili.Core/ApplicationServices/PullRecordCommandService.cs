using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameInterface.Jili.Client;
using Tgs.GameInterface.Jili.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Jili.Core.ApplicationServices;

internal class PullRecordCommandService(
	ILogger<PullRecordCommandService> logger,
	IOptions<GameRecordInfoOptions> options,
	IGameInterfaceJiliClient client,
	ICommandService<HandleRecordCommand> service) : ICommandService<PullRecordCommand>
{
	public async ValueTask ExecuteAsync(PullRecordCommand command, CancellationToken cancellationToken = default)
	{
		var page = 1;
		var pageLimit = options.Value.PageLimit;

		IEnumerable<GameRecordInfo> gameRecords;
		do
		{
			gameRecords = await client.GameRecordAsync(
			   apiUrl: command.ApiUrl,
			   agentId: command.AgentId,
			   agentKey: command.AgentKey,
			   startTime: command.StartTime,
			   endTime: command.EndTime,
			   page: page,
			   pageLimit: pageLimit,
			   cancellationToken: cancellationToken)
			   .ToArrayAsync(
			   cancellationToken: cancellationToken)
			   .ConfigureAwait(false)
			   ?? Enumerable.Empty<GameRecordInfo>();

			logger.LogDebug("LogOn:{logOn} Message: Settlement. GameRecords:{gameRecords}", DateTime.Now.ToString("o"), gameRecords);

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

			page++;
		} while (gameRecords.Any());
	}
}
