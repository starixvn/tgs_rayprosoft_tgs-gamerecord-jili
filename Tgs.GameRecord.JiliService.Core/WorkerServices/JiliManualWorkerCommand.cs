namespace Tgs.GameRecord.JiliService.Core.WorkerServices
{
    public record JiliManualWorkerCommand(
        Uri GameApiUrl,
        string GameProviderAgentId,
        string GameProviderAgentKey,
        DateTimeOffset StartTime,
        DateTimeOffset EndTime);
}
