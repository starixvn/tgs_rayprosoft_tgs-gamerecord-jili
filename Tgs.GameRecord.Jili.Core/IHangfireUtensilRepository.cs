using Tgs.GameRecord.Jili.Core.JobApplicationServices;

namespace Tgs.GameRecord.Jili.Core;

public interface IHangfireUtensilRepository
{
	public Task JobCheckAsync();

	public Task JobExecute30MinsAsync(JobExecute30MinsCommand source);

	public Task JobExecuteAsync(JobExecuteCommand source);

	public Task JobExecuteHourlyAsync(JobExecuteHourlyCommand source);

	public Task JobInitAsync();
}
