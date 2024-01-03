using Tgs.GameRecord.Jili.Core.JobApplicationServices;

namespace Tgs.GameRecord.Jili.Core;

public interface IHangfireUtensil
{
    public Task JobCheckAsync();

    public Task JobCheckExecuteAsync(JobCheckExecuteCommand source);

    public Task JobExecuteAsync(JobExecuteCommand source);

    public Task JobInitAsync();
}
