using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameRecord.Jili.Common;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Jili.Core.JobApplicationServices;

internal class JobExecuteCommandService : ICommandService<JobExecuteCommand>
{
    private readonly GameRecordInfoOptions _gameRecordInfo;
    private readonly double _intervalMinutes;
    private readonly ILogger<JobExecuteCommandService> _logger;
    private readonly IMessageSender _messageSender;

    public JobExecuteCommandService(
        ILogger<JobExecuteCommandService> logger,
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

    public ValueTask ExecuteAsync(JobExecuteCommand command, CancellationToken cancellationToken = default)
    {
        var endTime = DateTimeOffset.Parse(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:00.000z"));
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

        return _messageSender
            .PublishAsync(
            subject: JiliNatsConst.SchedulePublish,
            data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(parameters)),
            cancellationToken: cancellationToken);
    }
}
