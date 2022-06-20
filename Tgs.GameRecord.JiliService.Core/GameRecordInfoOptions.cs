namespace Tgs.GameRecord.JiliService.Core
{
    public record GameRecordInfoOptions()
    {
        public string GameProviderCode { get; set; } = default!;
        public decimal IntervalMinute { get; set; } = 5m;
        public int PageLimit { get; set; } = 500;
    }
}
