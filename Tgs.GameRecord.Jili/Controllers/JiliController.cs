using System.Text.Json;

using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.Jili.Common;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Jili.Controllers;

[Route("[controller]")]
[ApiController]
public class JokerController : ControllerBase
{
	/// <summary>
	/// Games the record manual asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost("manual")]
	public ValueTask GameRecordManualAsync(
		[FromServices] IMessageSender messageSender,
		JiliGameRecordMessage source)
		=> messageSender.PublishAsync(
			subject: JiliNatsConst.ManualPublish,
			data: JsonSerializer.SerializeToUtf8Bytes(source),
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Games the record schedule asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost("schedule")]
	public ValueTask GameRecordScheduleAsync(
		[FromServices] IMessageSender messageSender,
		JiliGameRecordMessage source)
		=> messageSender.PublishAsync(
			subject: JiliNatsConst.SchedulePublish,
			data: JsonSerializer.SerializeToUtf8Bytes(source),
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Games the record schedule mins asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost("schedule/mins")]
	public ValueTask GameRecordScheduleMinsAsync(
		[FromServices] IMessageSender messageSender,
		JiliGameRecordMessage source)
		=> messageSender.PublishAsync(
			subject: JiliNatsConst.ScheduleMinsPublish,
			data: JsonSerializer.SerializeToUtf8Bytes(source),
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Games the record schedule hourly asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost("schedule/hourly")]
	public ValueTask GameRecordScheduleHourlyAsync(
		[FromServices] IMessageSender messageSender,
		JiliGameRecordMessage source)
		=> messageSender.PublishAsync(
			subject: JiliNatsConst.ScheduleHourlyPublish,
			data: JsonSerializer.SerializeToUtf8Bytes(source),
			cancellationToken: HttpContext.RequestAborted);
}
