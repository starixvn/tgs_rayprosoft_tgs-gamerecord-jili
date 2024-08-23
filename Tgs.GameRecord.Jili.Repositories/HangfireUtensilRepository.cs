using Tgs.GameRecord.Jili.Core;

using Tgs.GameRecord.Jili.Core.JobApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Jili.Repositories;

internal class HangfireUtensilRepository(
	ICommandService<JobCheckCommand> jobCheckService,
	ICommandService<JobExecute30MinsCommand> jobExecute30MinsService,
	ICommandService<JobExecuteCommand> jobExecuteService,
	ICommandService<JobExecuteHourlyCommand> jobExecuteHourlService,
	ICommandService<JobInitCommand> jobInitservice) : IHangfireUtensilRepository
{
	public Task JobCheckAsync()
		=> jobCheckService.ExecuteAsync(new()).AsTask();

	public Task JobExecute30MinsAsync(JobExecute30MinsCommand source)
		=> jobExecute30MinsService.ExecuteAsync(source).AsTask();

	public Task JobExecuteAsync(JobExecuteCommand source)
		=> jobExecuteService.ExecuteAsync(source).AsTask();

	public Task JobExecuteHourlyAsync(JobExecuteHourlyCommand source)
		=> jobExecuteHourlService.ExecuteAsync(source).AsTask();

	public Task JobInitAsync()
		=> jobInitservice.ExecuteAsync(new()).AsTask();
}
