namespace Tgs.GameRecord.JiliService.Core;

public record GameRecordInfoOptions()
{
    public string GameProviderCode { get; set; } = default!;
    public int PageLimit { get; set; } = 10000;
}
