using Tgs.GameInterface.JiliService.Protos;

namespace Tgs.GameRecord.JiliService.Core.ApplicationServices;

public record ManualSettlementCommand(IEnumerable<GameInterfaceJiliServiceGamePullRecord> Records);
