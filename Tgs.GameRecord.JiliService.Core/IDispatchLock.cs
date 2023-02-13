using Tgs.GameRecord.JiliService.Core.DispatchServices;

namespace Tgs.GameRecord.JiliService.Core;

public interface IDispatchLock
{
    public Task<bool> LockingAsync(string key, ScheduleDispatchCommand command);

    public Task<bool> LockingAsync(string key, ManualDispatchCommand command);

    public Task<bool> UnlockAsync(string key);
}
