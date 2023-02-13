using Tgs.GameInterface.JiliService.Protos;

namespace Tgs.GameRecord.JiliService.Core.ApplicationServices;

public record SettlementCommand(IEnumerable<GameInterfaceJiliServiceGamePullRecord> Records);
