namespace Tgs.GameRecord.JiliManualService.ViewModels;

public record GameRecordQueryViewModel(
    Uri GameApiUrl,
    string GameProviderAgentId,
    string GameProviderAgentKey,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
