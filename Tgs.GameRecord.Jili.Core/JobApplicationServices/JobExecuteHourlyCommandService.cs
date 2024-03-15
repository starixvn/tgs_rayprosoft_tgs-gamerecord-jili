using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameRecord.Jili.Common;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Jili.Core.JobApplicationServices;

internal class JobExecuteHourlyCommandService : ICommandService<JobExecuteHourlyCommand>
{
    private readonly GameRecordInfoOptions _gameRecordInfo;
    private readonly double _intervalMinutes;
    private readonly ILogger<JobExecuteHourlyCommandService> _logger;
    private readonly IMessageSender _messageSender;

    public JobExecuteHourlyCommandService(
        ILogger<JobExecuteHourlyCommandService> logger,
        IOptions<GameRecordInfoOptions> options,
        IMessageSender messageSender)
    {
        _gameRecordInfo = options.Value;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
        _intervalMinutes = _gameRecordInfo.IntervalMinutesMax < _gameRecordInfo.IntervalMinutes
            ? _gameRecordInfo.IntervalMinutesMax
            : _gameRecordInfo.IntervalMinutesMinimum > _gameRecordInfo.IntervalMinutes
            ? _gameRecordInfo.IntervalMinutesMinimum
            : _gameRecordInfo.IntervalMinutes;
    }

    public ValueTask ExecuteAsync(JobExecuteHourlyCommand command, CancellationToken cancellationToken = default)
    {
        var currentTime = DateTimeOffset.UtcNow;
        var endTime = new DateTimeOffset(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, 00, currentTime.Offset).AddHours(-1);
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
            subject: JiliNatsConst.ScheduleHourlyPublish,
            data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(parameters)),
            cancellationToken: cancellationToken);
    }
}
