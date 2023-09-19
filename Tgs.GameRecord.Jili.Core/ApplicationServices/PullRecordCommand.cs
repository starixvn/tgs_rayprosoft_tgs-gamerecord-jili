namespace Tgs.GameRecord.Jili.Core.ApplicationServices;

public record PullRecordCommand(
    Uri ApiUrl,
    string AgentId,
    string AgentKey,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
