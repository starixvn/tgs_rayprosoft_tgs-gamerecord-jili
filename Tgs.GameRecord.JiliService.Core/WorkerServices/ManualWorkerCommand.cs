namespace Tgs.GameRecord.JiliService.Core.WorkerServices
{
    public record ManualWorkerCommand(
        Uri GameApiUrl,
        string GameProviderAgentId,
        string GameProviderAgentKey,
        DateTimeOffset StartTime,
        DateTimeOffset EndTime);
}
