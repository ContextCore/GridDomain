using System.Threading.Tasks;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Common.Configuration;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Logging {




    public class LogForAutoTestKitTests : TestKit
    {
        public LogForAutoTestKitTests(ITestOutputHelper output):base(new AutoTestNodeConfiguration(LogEventLevel.Verbose).ToStandAloneInMemorySystemConfig())
        {
            Sys.AttachSerilogLogging(new XUnitAutoTestLoggerConfiguration(output).CreateLogger());
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
        }

        [Fact(Skip = "Only for manual run")]
        public async Task ShouldLog_from_test_system_actor()
        {
            var actor = Sys.ActorOf(Props.Create(() => new TestLogActor()));
            await actor.Ask<TestMessage>(new TestMessage("ping"));
        }

        protected override void Dispose(bool disposing)
        {
            Sys?.Terminate().Wait();
            base.Dispose(disposing);
        }
    }
}