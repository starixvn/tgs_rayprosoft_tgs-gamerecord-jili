namespace Tgs.GameRecord.JiliService.Core.DispatchServices;

public record ManualDispatchCommand(
    Uri GameApiUrl,
    string GameProviderAgentId,
    string GameProviderAgentKey);
