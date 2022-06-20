namespace Tgs.GameRecord.JiliService
{
    public record ScheduleMessageModel(
        Uri GameApiUrl,
        string GameProviderAgentId,
        string GameProviderAgentKey);
}
