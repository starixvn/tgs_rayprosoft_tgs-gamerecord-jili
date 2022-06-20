using System.Text;
using System.Text.Json;
using Tgs.GameRecord.JiliService.Core.WorkerServices;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.JiliService
{
    public class ManualMessageHandler : IMessageProcessor
    {
        private readonly ICommandService<JiliWorkerCommand> _service;

        public ManualMessageHandler(ICommandService<JiliWorkerCommand> service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<ReadOnlyMemory<byte>> HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
        {
            var json = Encoding.UTF8.GetString(data.Span);
            var request = JsonSerializer.Deserialize<ManualMessageRequest>(json)!;

            var gameRecordCommand = new JiliWorkerCommand(
                GameApiUrl: request.GameApiUrl,
                GameProviderAgentId: request.GameProviderAgentId,
                GameProviderAgentKey: request.GameProviderAgentKey,
                StartTime: request.StartTime,
                EndTime: request.EndTime);

            await _service.ExecuteAsync(
                command: gameRecordCommand,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            var response = new ManualMessageResponse(true);
            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
        }
    }
}
