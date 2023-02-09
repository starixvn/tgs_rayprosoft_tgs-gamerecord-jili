namespace Tgs.GameRecord.JiliManualService;

public record ScheduleManualRequest(
    Uri GameApiUrl,
    string GameProviderAgentId,
    string GameProviderAgentKey);
