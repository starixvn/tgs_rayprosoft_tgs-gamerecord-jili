using Tgs.GameRecord.Jili.Core.ApplicationServices;

namespace Tgs.GameRecord.Jili.Core;

public interface IPullRecordDispatcher
{
	ValueTask PushCommandAsync(PullRecordCommand command, CancellationToken cancellationToken = default);
}
