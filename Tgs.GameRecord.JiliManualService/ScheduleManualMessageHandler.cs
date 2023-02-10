using System.Text;
using System.Text.Json;
using Tgs.GameRecord.JiliService.Core.WorkerServices;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.JiliManualService;

public class ScheduleManualMessageHandler : IMessageHandler
{
    private readonly ILogger<ScheduleManualMessageHandler> _logger;
    private readonly ICommandService<JiliManualWorkerCommand> _service;

    public ScheduleManualMessageHandler(
        ILogger<ScheduleManualMessageHandler> logger,
        ICommandService<JiliManualWorkerCommand> service)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public ValueTask HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        var json = Encoding.UTF8.GetString(data.Span);
        var request = JsonSerializer.Deserialize<ScheduleManualRequest>(json) ??
            throw new NullReferenceException("The request data resolves to a null value");

        _logger.LogInformation("LogOn:{logOn} Information Request:{receive}", DateTime.Now.ToString("o"), request);

        var endTime = DateTimeOffset.ParseExact(
                        DateTimeOffset.UtcNow.AddHours(-1).ToString(@"yyyy-MM-dd\THH:mm:00K"),
                        @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind);

        var startTime = endTime.AddMinutes(-3);

        var command = new JiliManualWorkerCommand(
            GameApiUrl: request.GameApiUrl,
            GameProviderAgentId: request.GameProviderAgentId,
            GameProviderAgentKey: request.GameProviderAgentKey,
            StartTime: startTime,
            EndTime: endTime);

        _logger.LogInformation("LogOn:{logOn} command:{command}", DateTime.Now.ToString("o"), command);

        return _service.ExecuteAsync(
            command: command,
            cancellationToken: cancellationToken);

    }
}
