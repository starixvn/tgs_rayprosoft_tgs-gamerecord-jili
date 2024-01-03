using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameInterface.Jili.Client;
using Tgs.GameInterface.JiliService.Protos;
using Tgs.GameRecord.Jili.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Jili.Core.UnitTest.ApplicationServices;

public class PullRecordCommandServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<PullRecordCommandService>.Instance;
        var options = Options.Create(new GameRecordInfoOptions
        {
            GameProviderCode = "JILITF",
            GameProviderConfiguration = "JILITFConfiguration",
            HangfirePrefix = "gamerecord-jili-hangfire:",
            HangfireLocking = "gamerecord-jili-locking:",
            IntervalMinutes = 5,
            IntervalMinutesMax = 10,
            IntervalMinutesMinimum = 1,
            PageLimit = 10000
        });
        var fakeClient = Substitute.For<IGameInterfaceJiliClient>();
        var fakeService = Substitute.For<ICommandService<HandleRecordCommand>>();

        var command = new PullRecordCommand(
            ApiUrl: new("http://localhost/"),
            AgentId: "agentId",
            AgentKey: "agentKey",
            StartTime: DateTimeOffset.UtcNow.AddMinutes(-10),
            EndTime: DateTimeOffset.UtcNow);

        _ = fakeClient.GetGamePullRecordAsync(
            apiUrl: Arg.Is(command.ApiUrl),
            agentId: Arg.Is(command.AgentId),
            agentKey: Arg.Is(command.AgentKey),
            startTime: Arg.Is(command.StartTime),
            endTime: Arg.Is(command.EndTime),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(new GamePullRecordReply { }));

        var service = new PullRecordCommandService(
            fakeLogger,
            options,
            fakeClient,
            fakeService);

        await service.ExecuteAsync(command: command);
    }
}
