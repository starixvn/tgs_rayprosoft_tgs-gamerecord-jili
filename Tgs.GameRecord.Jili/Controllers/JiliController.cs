using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.Jili.Common;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Jili.Controllers;

[ApiController]
[Route("JILI")]
public class JiliController : ControllerBase
{
    /// <summary>
    /// Manuals the publish asynchronous.
    /// </summary>
    /// <param name="messageSender">The message sender.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///         {
    ///             "apiUrl":"https://uat-wb-api.jlfafafa2.com/",
    ///             "gameProviderAgentId":"PG_GPI_CNY",
    ///             "gameProviderAgentKey":"82af5971ae233eaa003b774be4079e7902ea7ff4",
    ///             "startTime": "2022-09-07T18:50:00+08:00",
    ///             "endTime": "2022-09-07T18:55:00+08:00"
    ///         }
    /// </remarks>
    [HttpPost]
    public ValueTask ManualPublishAsync(
        [FromServices] IMessageSender messageSender,
        JiliGameRecordMessage source)
        => messageSender.PublishAsync(
            subject: JiliNatsConst.ManualPublish,
            data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(source)),
            cancellationToken: HttpContext.RequestAborted);
}
