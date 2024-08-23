using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.Jili.Common;

public record JiliGameRecordMessage
{
	/// <summary>
	/// The API URL
	/// </summary>
	[Required]
	[DataType(DataType.Url)]
	[DefaultValue(typeof(Uri), "https://uat-wb-api.jlfafafa2.com/")]
	public Uri ApiUrl { get; init; } = default!;
	/// <summary>
	/// The agent identifier
	/// </summary>
	[Required]
	[DefaultValue(typeof(string), "PG_GPI_CNY")]
	public string AgentId { get; init; } = default!;
	/// <summary>
	/// The agent key
	/// </summary>
	[Required]
	[DefaultValue(typeof(string), "82af5971ae233eaa003b774be4079e7902ea7ff4")]
	public string AgentKey { get; init; } = default!;
	/// <summary>
	/// The start time
	/// </summary>
	[Required]
	[DataType(DataType.DateTime)]
	[DefaultValue(typeof(DateTimeOffset), "2022-09-07T18:50:00+08:00")]
	public DateTimeOffset StartTime { get; init; }
	/// <summary>
	/// The end time
	/// </summary>
	[Required]
	[DataType(DataType.DateTime)]
	[DefaultValue(typeof(DateTimeOffset), "2022-09-07T18:55:00+08:00")]
	public DateTimeOffset EndTime { get; init; }
}
