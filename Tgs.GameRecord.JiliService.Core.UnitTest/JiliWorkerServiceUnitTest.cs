using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameInterface.JiliService.Client;
using Tgs.GameInterface.JiliService.Protos;
using Tgs.GameRecord.JiliService.Core.ApplicationServices;
using Tgs.GameRecord.JiliService.Core.WorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.JiliService.Core.UnitTest
{
    [TestClass]
    public class JiliWorkerServiceUnitTest
    {
        [TestMethod]
        public async Task Normal_Process()
        {
            var fakeGameInterfaceJiliServiceGrpc = Substitute.For<IGameInterfaceJiliServiceGrpc>();
            var fakeLogger = NullLogger<JiliWorkerService>.Instance;
            var fakeGamePullRecordCommand = Substitute.For<ICommandService<GamePullRecordCommand>>();
            var fakeUtensil = Substitute.For<IUtensil>();
            var gameRecordInfoOptions = new GameRecordInfoOptions(
                GameProviderCode: "JILITF",
                GameProviderConfig: "JILITFConfiguration",
                IntervalMinute: 5,
                PageLimit: 500);
            var options = Options.Create(gameRecordInfoOptions);
            var command = new JiliWorkerCommand(
                GameApiUrl: new("http://localhost/"),
                GameProviderAgentId: "testAgentId",
                GameProviderAgentKey: "testCert",
                StartTime: DateTime.UtcNow.AddMinutes(-5),
                EndTime: DateTime.UtcNow);

            _ = fakeUtensil.GetGameProviderIdAsync(gameRecordInfoOptions.GameProviderCode).Returns(Guid.NewGuid());

            var gamePullRecordReply = new GameInterfaceJiliServiceGamePullRecordReply();
            _ = fakeGameInterfaceJiliServiceGrpc
                .GetGamePullRecordAsync(
                    Arg.Is(command.GameApiUrl),
                    Arg.Is(command.GameProviderAgentId),
                    Arg.Is(command.GameProviderAgentKey),
                    Arg.Any<DateTimeOffset>(),
                    Arg.Any<DateTimeOffset>()).Returns(gamePullRecordReply);

            var service = new JiliWorkerService(
                options,
                fakeGameInterfaceJiliServiceGrpc,
                fakeLogger,
                fakeUtensil,
                fakeGamePullRecordCommand);

            await service.ExecuteAsync(command).ConfigureAwait(false);
        }
    }
}
