using Tgs.GameRecord.Jili.Core.JobApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Jili.Core;

internal class HangfireUtensil(
    ICommandService<JobCheckCommand> jobCheckService,
    ICommandService<JobCheckExecuteCommand> jobCheckExecuteService,
    ICommandService<JobExecuteCommand> jobExecuteService,
    ICommandService<JobInitCommand> jobInitservice) : IHangfireUtensil
{
    public Task JobCheckAsync()
        => jobCheckService.ExecuteAsync(new()).AsTask();

    public Task JobCheckExecuteAsync(JobCheckExecuteCommand source)
        => jobCheckExecuteService.ExecuteAsync(source).AsTask();

    public Task JobExecuteAsync(JobExecuteCommand source)
        => jobExecuteService.ExecuteAsync(source).AsTask();

    public Task JobInitAsync()
        => jobInitservice.ExecuteAsync(new()).AsTask();
}
