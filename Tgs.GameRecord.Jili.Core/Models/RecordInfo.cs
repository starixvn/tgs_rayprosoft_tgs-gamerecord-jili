using Tgs.GameInterface.JiliService.Protos;

namespace Tgs.GameRecord.Jili.Core.Models;

public record RecordInfo(
	Guid GameProviderId,
	string GameProviderCode,
	string ExternalRoundId,
	int GameId,
	string ExternalGameId,
	string PlayerCode,
	Guid WalletId,
	string CurrencyCode,
	Guid BrandId,
	Guid GameProviderConfigurationId,
	GamePullRecordStatus Status,
	DateTimeOffset ExternalCreatedOn,
	DateTimeOffset? ExternalClosedOn,
	decimal Bet,
	decimal ValidBet,
	decimal Settle,
	decimal Commission,
	decimal Turnover,
	decimal WinLoss,
	string AdditionalData);
