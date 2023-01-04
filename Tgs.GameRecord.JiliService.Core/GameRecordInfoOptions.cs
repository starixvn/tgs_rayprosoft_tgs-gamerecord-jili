namespace Tgs.GameRecord.JiliService.Core;

public record GameRecordInfoOptions()
{
    public string GameProviderCode { get; set; } = default!;

    public double IntervalMinute { get; set; } = 5d;
    public double LockMinute { get; set; } = 4d;
    public int PageLimit { get; set; } = 500;
}
