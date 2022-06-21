namespace Tgs.GameRecord.JiliService
{
    public record ManualMessageRequest(
        Uri GameApiUrl,
        string GameProviderAgentId,
        string GameProviderAgentKey,
        DateTimeOffset StartTime,
        DateTimeOffset EndTime);
}
