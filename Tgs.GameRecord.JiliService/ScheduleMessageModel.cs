namespace Tgs.GameRecord.JiliService
{
    public record ScheduleMessageModel(
        Uri GameRecordApiUrl,
        string GameProviderAgentId,
        string GameProviderAgentKey);
}
