using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.Jili.Common;

public record JiliGameRecordMessage
{
	/// <summary>
	/// The API URL
	/// </summary>
	[Required]
	[DataType(DataType.Url)]
	public Uri ApiUrl { get; init; } = default!;
	/// <summary>
	/// The agent identifier
	/// </summary>
	[Required]
	public string AgentId { get; init; } = default!;
	/// <summary>
	/// The agent key
	/// </summary>
	[Required]
	public string AgentKey { get; init; } = default!;
	/// <summary>
	/// The start time
	/// </summary>
	[Required]
	[DataType(DataType.DateTime)]
	public DateTimeOffset StartTime { get; init; }
	/// <summary>
	/// The end time
	/// </summary>
	[Required]
	[DataType(DataType.DateTime)]
	public DateTimeOffset EndTime { get; init; }
}
