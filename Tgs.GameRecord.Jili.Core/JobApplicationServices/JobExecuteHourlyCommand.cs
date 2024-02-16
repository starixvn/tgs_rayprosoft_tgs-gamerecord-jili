namespace Tgs.GameRecord.Jili.Core.JobApplicationServices;

public record JobExecuteHourlyCommand(
    string GameProviderCode,
    string Name,
    Uri ApiUrl,
    string AgentId,
    string AgentKey);