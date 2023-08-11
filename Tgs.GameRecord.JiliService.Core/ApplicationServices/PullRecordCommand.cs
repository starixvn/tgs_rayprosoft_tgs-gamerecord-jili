namespace Tgs.GameRecord.JiliService.Core.ApplicationServices;

public record PullRecordCommand(
    Uri ApiUrl,
    string AgentId,
    string AgentKey,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
