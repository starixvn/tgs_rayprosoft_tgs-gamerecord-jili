using System.Text;
using System.Text.Json;
using Tgs.GameRecord.JiliService.Core.DispatchServices;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.JiliManualService;

public class ScheduleManualMessageHandler : IMessageHandler
{
    private readonly ILogger<ScheduleManualMessageHandler> _logger;
    private readonly ICommandService<ManualDispatchCommand> _service;

    public ScheduleManualMessageHandler(
        ILogger<ScheduleManualMessageHandler> logger,
        ICommandService<ManualDispatchCommand> service)
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

        var command = new ManualDispatchCommand(
            GameApiUrl: request.GameApiUrl,
            GameProviderAgentId: request.GameProviderAgentId,
            GameProviderAgentKey: request.GameProviderAgentKey);

        _logger.LogInformation("LogOn:{logOn} command:{command}", DateTime.Now.ToString("o"), command);

        return _service.ExecuteAsync(
            command: command,
            cancellationToken: cancellationToken);
    }
}
