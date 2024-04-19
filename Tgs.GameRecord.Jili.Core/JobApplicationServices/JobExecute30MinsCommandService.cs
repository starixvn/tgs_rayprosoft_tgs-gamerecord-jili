using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameRecord.Jili.Common;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Jili.Core.JobApplicationServices;

internal class JobExecute30MinsCommandService : ICommandService<JobExecute30MinsCommand>
{
	private readonly GameRecordInfoOptions _gameRecordInfo;
	private readonly double _intervalMinutes;
	private readonly ILogger<JobExecuteCommandService> _logger;
	private readonly TimeProvider _timeProvider;
	private readonly IMessageSender _messageSender;

	public JobExecute30MinsCommandService(
		ILogger<JobExecuteCommandService> logger,
		IOptions<GameRecordInfoOptions> options,
		TimeProvider timeProvider,
		IMessageSender messageSender)
	{
		_gameRecordInfo = options.Value;
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
		_messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
		_intervalMinutes = _gameRecordInfo.IntervalMinutesMax < _gameRecordInfo.IntervalMinutes
			? _gameRecordInfo.IntervalMinutesMax
			: _gameRecordInfo.IntervalMinutesMinimum > _gameRecordInfo.IntervalMinutes
			? _gameRecordInfo.IntervalMinutesMinimum
			: _gameRecordInfo.IntervalMinutes;
	}

	public ValueTask ExecuteAsync(JobExecute30MinsCommand command, CancellationToken cancellationToken = default)
	{
		var currentTime = _timeProvider.GetUtcNow();
		var endTime = new DateTimeOffset(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, 00, currentTime.Offset).AddMinutes(-30);
		var startTime = endTime.AddMinutes(_intervalMinutes * -1);

		var parameters = new JiliGameRecordMessage
		{
			ApiUrl = command.ApiUrl,
			AgentId = command.AgentId,
			AgentKey = command.AgentKey,
			StartTime = startTime,
			EndTime = endTime
		};

		_logger.LogInformation("LogOn:{logOn} Parameters:{parameters}", DateTime.Now.ToString("u"), parameters);

		return _messageSender.PublishAsync<ReadOnlyMemory<byte>>(
			subject: JiliNatsConst.ScheduleMinsPublish,
			data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(parameters)),
			cancellationToken: cancellationToken);
	}
}
