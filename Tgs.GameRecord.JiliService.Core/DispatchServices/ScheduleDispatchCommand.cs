namespace Tgs.GameRecord.JiliService.Core.DispatchServices;

public record ScheduleDispatchCommand(
    Uri GameApiUrl,
    string GameProviderAgentId,
    string GameProviderAgentKey);
