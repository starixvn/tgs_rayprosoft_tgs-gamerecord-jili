using Tgs.GameInterface.JiliService.Protos;
using Tgs.GameRecord.JiliService.Core.Models;

namespace Tgs.GameRecord.JiliService.Core;

public interface IGameRecordUtensil
{
    public ValueTask<RecordInfo> SettlementConvertRecordAsync(
        GamePullRecord source,
        CancellationToken cancellationToken = default);
}
