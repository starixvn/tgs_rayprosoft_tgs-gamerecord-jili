using System.Text;
using System.Text.Json;
using Tgs.GameRecord.JiliService.Core.DispatchServices;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.JiliService;

internal class ScheduleMessageHandler : IMessageHandler
{
    private readonly ILogger<ScheduleMessageHandler> _logger;
    private readonly ICommandService<ScheduleDispatchCommand> _service;

    public ScheduleMessageHandler(
        ILogger<ScheduleMessageHandler> logger,
        ICommandService<ScheduleDispatchCommand> service
        )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public ValueTask HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        var json = Encoding.UTF8.GetString(data.Span);
        var request = JsonSerializer.Deserialize<ScheduleMessageModel>(json) ??
            throw new NullReferenceException("The request data resolves to a null value");

        _logger.LogInformation("LogOn:{logOn} Information Request:{receive}", DateTime.Now.ToString("o"), request);

        var command = new ScheduleDispatchCommand(
            GameApiUrl: request.GameApiUrl,
            GameProviderAgentId: request.GameProviderAgentId,
            GameProviderAgentKey: request.GameProviderAgentKey);

        return _service.ExecuteAsync(
            command: command,
            cancellationToken: cancellationToken);
    }
}
