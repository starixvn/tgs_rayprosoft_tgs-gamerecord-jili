namespace Tgs.GameRecord.JiliService.Core
{
    public record GameRecordInfoOptions(
        string GameProviderCode,
        string GameProviderConfig,
        decimal IntervalMinute = 5m,
        int PageLimit = 500);
}
