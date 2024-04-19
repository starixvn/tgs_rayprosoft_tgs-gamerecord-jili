namespace Tgs.GameRecord.Jili.Core.JobApplicationServices;

public record JobExecuteCommand(
	string GameProviderCode,
	string Name,
	Uri ApiUrl,
	string AgentId,
	string AgentKey);
