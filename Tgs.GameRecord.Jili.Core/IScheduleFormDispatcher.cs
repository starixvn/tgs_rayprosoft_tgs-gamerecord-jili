namespace Tgs.GameRecord.Jili.Core;

public interface IScheduleFormDispatcher
{
    ValueTask ExecuteAsync(
        string gameProviderCode,
        long id,
        CancellationToken cancellationToken = default);
}
