namespace Tgs.GameRecord.Jili.Core;

public record GameRecordInfoOptions()
{
	public string GameProviderCode { get; set; } = default!;
	public string GameProviderConfiguration { get; init; } = default!;
	public string HangfirePrefix { get; set; } = "gamerecord-jili-hangfire:";
	public string HangfireLocking { get; set; } = "gamerecord-jili-locking:";
	public double IntervalMinutes { get; init; } = 5;
	public double IntervalMinutesMax { get; init; } = 10;
	public double IntervalMinutesMinimum { get; init; } = 1;
	public int PageLimit { get; set; } = 10000;
}
