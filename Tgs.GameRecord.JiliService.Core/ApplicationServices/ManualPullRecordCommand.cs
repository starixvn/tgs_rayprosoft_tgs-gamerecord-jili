namespace Tgs.GameRecord.JiliService.Core.ApplicationServices;

public record ManualPullRecordCommand(
    Uri ApiUrl,
    string AgentId,
    string AgentKey,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
