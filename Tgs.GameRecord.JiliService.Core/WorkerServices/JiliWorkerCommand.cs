namespace Tgs.GameRecord.JiliService.Core.WorkerServices
{
    public record JiliWorkerCommand(
        Uri GameApiUrl,
        string GameProviderAgentId,
        string GameProviderAgentKey,
        DateTimeOffset StartTime,
        DateTimeOffset EndTime);
}
