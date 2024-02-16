using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgs.GameRecord.Jili.Core.JobApplicationServices;

public record JobExecute30MinsCommand(
    string GameProviderCode,
    string Name,
    Uri ApiUrl,
    string AgentId,
    string AgentKey);
