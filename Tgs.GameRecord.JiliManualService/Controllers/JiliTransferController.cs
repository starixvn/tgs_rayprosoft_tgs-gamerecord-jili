using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.JiliManualService.ViewModels;
using Tgs.GameRecord.JiliService.Core.WorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.JiliManualService.Controllers;

[ApiController]
[Route("jilitf")]
public class JiliTransferController : ControllerBase
{
    [HttpPost]
    public ValueTask ExecuteAsync(
        [FromServices] ICommandService<ScheduleWorkerCommand> service,
        GameRecordQueryViewModel model)
        => service.ExecuteAsync(new ScheduleWorkerCommand(
            GameApiUrl: model.GameApiUrl,
            GameProviderAgentId: model.GameProviderAgentId,
            GameProviderAgentKey: model.GameProviderAgentKey,
            StartTime: model.StartTime,
            EndTime: model.EndTime));
}
