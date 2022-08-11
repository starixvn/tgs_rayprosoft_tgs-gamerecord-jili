using Tgs.GameInterface.JiliService.Protos;

namespace Tgs.GameRecord.JiliService.Core.ApplicationServices
{
    public record GamePullRecordBatchCommand(Guid GameProviderId, string GameProviderCode, IEnumerable<GameInterfaceJiliServiceGamePullRecord> Records);
}
