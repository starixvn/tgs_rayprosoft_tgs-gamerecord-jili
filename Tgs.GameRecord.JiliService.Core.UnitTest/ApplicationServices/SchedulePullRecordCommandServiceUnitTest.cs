using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameInterface.Jili.Client;
using Tgs.GameInterface.JiliService.Protos;
using Tgs.GameRecord.JiliService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.JiliService.Core.UnitTest.ApplicationServices;

public class SchedulePullRecordCommandServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<SchedulePullRecordCommandService>.Instance;
        var fakeClient = Substitute.For<IGameInterfaceJiliClient>();
        var fakeGameRecordUtensil = Substitute.For<IGameRecordUtensil>();
        var fakeService = Substitute.For<ICommandService<ScheduleHandleRecordCommand>>();

        var command = new SchedulePullRecordCommand(
            ApiUrl: new("http://localhost/"),
            AgentId: "agentId",
            AgentKey: "agentKey",
            StartTime: DateTimeOffset.UtcNow.AddMinutes(-10),
            EndTime: DateTimeOffset.UtcNow);

        _ = fakeClient
            .GetGamePullRecordAsync(
                apiUrl: Arg.Is(command.ApiUrl),
                agentId: Arg.Is(command.AgentId),
                agentKey: Arg.Is(command.AgentKey),
                startTime: Arg.Is(command.StartTime),
                endTime: Arg.Is(command.EndTime),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(new GamePullRecordReply { }));

        var service = new SchedulePullRecordCommandService(
            fakeLogger,
            fakeClient,
            fakeGameRecordUtensil,
            fakeService);

        await service
            .ExecuteAsync(
                command: command)
            .ConfigureAwait(false);
    }
}
