using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Common.Configuration;
using Serilog.Core;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Logging {
    public class LogForStandaloneActorSystemTest : IDisposable
    {
        private Logger _logger;
        public readonly ActorSystem Sys;

        public LogForStandaloneActorSystemTest(ITestOutputHelper output)
        {
            _logger = new XUnitAutoTestLoggerConfiguration(output).CreateLogger();
            Sys = new AutoTestNodeConfiguration().CreateInMemorySystem();
            Sys.AttachSerilogLogging(_logger);
        }

        [Fact(Skip = "Only for manual run")]
        public void All_levels_should_be_enabled()
        {
            Assert.True(Sys.Log.IsDebugEnabled);
            Assert.True(Sys.Log.IsErrorEnabled);
            Assert.True(Sys.Log.IsInfoEnabled);
            Assert.True(Sys.Log.IsWarningEnabled);

            Sys.Log.Info(Sys.Settings.ToString());
        }

        [Fact(Skip = "Only for manual run")]
        public async Task ShouldLog_from_gridNode_actor()
        {
            var actor = Sys.ActorOf(Props.Create(() => new TestLogActor()), "testLoggingActor");
            await actor.Ask<TestMessage>(new TestMessage("ping"));
            await Task.Delay(500);
        }

        [Fact(Skip = "Only for manual run")]
        public async Task ShouldLog_from_test_system_actor()
        {
            var actor = Sys.ActorOf(Props.Create(() => new TestLogActor()));
            await actor.Ask<TestMessage>(new TestMessage("ping"));
            await Task.Delay(500);
        }

        public void Dispose()
        {
            Sys?.Dispose();
        }
    }
}