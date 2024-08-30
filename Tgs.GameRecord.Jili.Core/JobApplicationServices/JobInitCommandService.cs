using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Hangfire;
using Hangfire.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Jili.Core.JobApplicationServices;

internal class JobInitCommandService(
	ILogger<JobInitCommandService> logger,
	IOptions<GameRecordInfoOptions> options,
	IHangfireLockRepository hangfireLock,
	IGameCommandClient client) : ICommandService<JobInitCommand>
{
	private readonly GameRecordInfoOptions _gameRecordInfo = options.Value;

	private readonly RecurringJobOptions _recurringJobOptions = new()
	{
		TimeZone = TimeZoneInfo.FindSystemTimeZoneById(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "China Standard Time" : "Asia/Shanghai")
	};

	public async ValueTask ExecuteAsync(JobInitCommand command, CancellationToken cancellationToken = default)
	{
		try
		{
			if (!await hangfireLock.LockingAsync(
				application: typeof(JobInitCommandService).Name,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false))
			{
				logger.LogWarning("{logWarning}",
					JsonSerializer.Serialize(new
					{
						LogOn = DateTime.Now.ToString("u"),
						Message = "This game provider configuration schedule has been initialized."
					}));
				return;
			}

			using (var connection = JobStorage.Current.GetConnection())
			{
				foreach (var recurringJob in connection.GetRecurringJobs())
				{
					RecurringJob.RemoveIfExists(recurringJob.Id);
				}
			}

			var settingConfiguration = await client
				.SettingGetConfigurationStringAsync(
				key: _gameRecordInfo.GameProviderConfiguration,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false)
				?? throw new Exception($"This game product setting does not exist.({_gameRecordInfo.GameProviderConfiguration})");

			var settingConfigurationJson = JsonNode.Parse(settingConfiguration)
				?? throw new Exception($"This game product setting does not exist.({_gameRecordInfo.GameProviderConfiguration})");

			var gameProviderConfigs = await client
				.GameProviderConfigurationQueryAsync(
				gameProviderCode: _gameRecordInfo.GameProviderCode,
				cancellationToken: cancellationToken)
				.ToArrayAsync(cancellationToken: cancellationToken)
				.ConfigureAwait(false)
				?? Enumerable.Empty<GameProviderConfigInfo>();

			if (gameProviderConfigs == null || !gameProviderConfigs.Any())
			{
				return;
			}

			var gameProviderAgentMapper = new Dictionary<string, JobExecuteCommand>();

			foreach (var gameProviderConfig in gameProviderConfigs)
			{
				if (string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData) || gameProviderConfig.Status == GameProviderConfigStatus.Inactive)
				{
					continue;
				}

				var configurationDataJson = JsonNode.Parse(gameProviderConfig.ConfigurationData)
					?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfig.GameProviderConfigId})");

				var apiUrl = new Uri(configurationDataJson["ApiUrl"]?.ToString()
					?? settingConfigurationJson["ApiUrl"]?.ToString()
					?? throw new Exception($"Required parameters(ApiUrl)"));
				var agentId = configurationDataJson["AgentId"]?.ToString()
					?? settingConfigurationJson["AgentId"]?.ToString()
					?? throw new Exception($"Required parameters(AgentId)");
				var agentKey = configurationDataJson["AgentKey"]?.ToString()
					?? settingConfigurationJson["AgentKey"]?.ToString()
					?? throw new Exception($"Required parameters(AgentKey)");

				if (gameProviderAgentMapper.ContainsKey(agentId))
				{
					continue;
				}

				var jobExecuteCommand = new JobExecuteCommand(
					GameProviderCode: _gameRecordInfo.GameProviderCode,
					Name: gameProviderConfig.Name,
					ApiUrl: apiUrl,
					AgentId: agentId,
					AgentKey: agentKey);

				var jobExecute30MinsCommand = new JobExecute30MinsCommand(
					GameProviderCode: _gameRecordInfo.GameProviderCode,
					Name: gameProviderConfig.Name,
					ApiUrl: apiUrl,
					AgentId: agentId,
					AgentKey: agentKey);

				var jobExecuteHourlyCommand = new JobExecuteHourlyCommand(
					GameProviderCode: _gameRecordInfo.GameProviderCode,
					Name: gameProviderConfig.Name,
					ApiUrl: apiUrl,
					AgentId: agentId,
					AgentKey: agentKey);

				gameProviderAgentMapper.Add(agentId, jobExecuteCommand);

				var recurringJobId = $"{gameProviderConfig.Name}|{agentId}|{gameProviderConfig.GameProviderConfigId}";

				RecurringJob.AddOrUpdate<IHangfireUtensilRepository>(
					recurringJobId: recurringJobId,
					methodCall: service => service.JobExecuteAsync(jobExecuteCommand),
					cronExpression: Cron.Minutely(),
					options: _recurringJobOptions);

				RecurringJob.AddOrUpdate<IHangfireUtensilRepository>(
					recurringJobId: string.Concat(recurringJobId, "|30Mins"),
					methodCall: service => service.JobExecute30MinsAsync(jobExecute30MinsCommand),
					cronExpression: Cron.Minutely(),
					options: _recurringJobOptions);
				/*
				RecurringJob.AddOrUpdate<IHangfireUtensil>(
					recurringJobId: string.Concat(recurringJobId, "|Hourly"),
					methodCall: service => service.JobExecuteHourlyAsync(jobExecuteHourlyCommand),
					cronExpression: Cron.Minutely(),
					options: _recurringJobOptions);
				*/
				//初始化強制第一次處理
				_ = BackgroundJob.Enqueue<IHangfireUtensilRepository>(service => service.JobExecuteAsync(jobExecuteCommand));
			}
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "{logError}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Message = "The game provider configuration schedule initialization failed."
				}));
		}
		finally
		{
			_ = BackgroundJob.Schedule<IHangfireUtensilRepository>(service => service.JobCheckAsync(), TimeSpan.FromMinutes(1));
		}
	}
}
