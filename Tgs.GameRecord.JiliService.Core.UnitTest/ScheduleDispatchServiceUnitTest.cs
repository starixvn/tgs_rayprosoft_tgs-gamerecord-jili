using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameRecord.JiliService.Core.DispatchServices;
using Tgs.GameRecord.JiliService.Core.WorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.JiliService.Core.UnitTest;

[TestClass]
public class ScheduleDispatchServiceUnitTest
{
    [TestMethod]
    public async Task Locking()
    {
        var gameRecordInfo = Options.Create(new GameRecordInfoOptions
        {
            GameProviderCode = "JILITF",
            IntervalMinutes = 5d,
            IntervalMinutesMax = 10d,
            IntervalMinutesMinimum = 1d,
            LockMinute = 4,
            PageLimit = 500
        });
        var fakeLogger = NullLogger<ScheduleDispatchService>.Instance;
        var fakeDispatchLock = Substitute.For<IDispatchLock>();
        var fakeCommandService = Substitute.For<ICommandService<ScheduleWorkerCommand>>();

        var command = new ScheduleDispatchCommand(
            new("http://localhost/"),
            "test",
            Guid.NewGuid().ToString("N"));

        var key = $"gamerecord:jili:dispatch:schedule:{command.GameProviderAgentId}";
        _ = fakeDispatchLock
            .LockingAsync(
                key: Arg.Is(key),
                command: Arg.Is(command))
            .Returns(true);

        var service = new ScheduleDispatchService(
            fakeLogger,
            gameRecordInfo,
            fakeDispatchLock,
            fakeCommandService);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        _ = fakeDispatchLock
           .Received(1)
           .UnlockAsync(key: key);
    }

    [TestMethod]
    public async Task Unlocked()
    {
        var gameRecordInfo = Options.Create(new GameRecordInfoOptions
        {
            GameProviderCode = "JILITF",
            IntervalMinutes = 5d,
            IntervalMinutesMax = 10d,
            IntervalMinutesMinimum = 1d,
            LockMinute = 4,
            PageLimit = 500
        });
        var fakeLogger = NullLogger<ScheduleDispatchService>.Instance;
        var fakeDispatchLock = Substitute.For<IDispatchLock>();
        var fakeCommandService = Substitute.For<ICommandService<ScheduleWorkerCommand>>();

        var command = new ScheduleDispatchCommand(
            new("http://localhost/"),
            "test",
            Guid.NewGuid().ToString("N"));

        var key = $"gamerecord:jili:dispatch:schedule:{command.GameProviderAgentId}";
        _ = fakeDispatchLock
            .LockingAsync(
                key: Arg.Is(key),
                command: Arg.Is(command))
            .Returns(false);

        var service = new ScheduleDispatchService(
            fakeLogger,
            gameRecordInfo,
            fakeDispatchLock,
            fakeCommandService);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        _ = fakeDispatchLock
           .Received(0)
           .UnlockAsync(key: key);
    }
}
