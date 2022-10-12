namespace Tgs.GameRecord.JiliManualService;

public record ManualMessageRequest(
    Uri GameApiUrl,
    string GameProviderAgentId,
    string GameProviderAgentKey,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
