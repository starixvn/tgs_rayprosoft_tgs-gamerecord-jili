namespace Tgs.GameRecord.JiliService.Core.ApplicationServices;

public record SchedulePullRecordCommand(
    Uri ApiUrl,
    string AgentId,
    string AgentKey,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
