using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.JiliService.Core.WorkerServices;
using Tgs.GameRecord.JiliService.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.JiliService.Controllers;

[ApiController]
[Route("jilitf")]
public class JiliTransferController : ControllerBase
{
    [HttpPost]
    public ValueTask ExecuteAsync(
        [FromServices] ICommandService<JiliWorkerCommand> service,
        GameRecordQueryViewModel model)
        => service.ExecuteAsync(new JiliWorkerCommand(
            GameApiUrl: model.GameApiUrl,
            GameProviderAgentId: model.GameProviderAgentId,
            GameProviderAgentKey: model.GameProviderAgentKey,
            StartTime: model.StartTime,
            EndTime: model.EndTime));
}
