namespace Tgs.GameRecord.JiliService.Core.ApplicationServices
{
    public record GameRoundBatchCommand(IEnumerable<GameRecord> GameRecords);
}
