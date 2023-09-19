namespace Tgs.GameRecord.Jili.Core;

public record GameRecordInfoOptions()
{
    public string GameProviderCode { get; set; } = default!;
    public string GameProviderConfiguration { get; init; } = default!;

    public string GameProviderHangfire { get; init; } = default!;

    public double IntervalMinutes { get; init; } = 3;

    public double IntervalMinutesMax { get; init; } = 15;

    public double IntervalMinutesMinimum { get; init; } = 1;
    public int PageLimit { get; set; } = 10000;
}
