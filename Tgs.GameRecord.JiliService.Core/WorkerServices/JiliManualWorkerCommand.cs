using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgs.GameRecord.JiliService.Core.WorkerServices
{
    public record JiliManualWorkerCommand(
        Uri GameApiUrl,
        string GameProviderAgentId,
        string GameProviderAgentKey,
        DateTimeOffset StartTime,
        DateTimeOffset EndTime);
}
