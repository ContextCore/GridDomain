using System.Threading.Tasks;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit {
    public class LogForTestKitTests : TestKit
    {
        public LogForTestKitTests(ITestOutputHelper output)
        {
            Sys.AttachSerilogLogging(new XUnitAutoTestLoggerConfiguration(output).CreateLogger());
        }

        [Fact(Skip = "Only for manual run")]
        public async Task ShouldLog_from_gridNode_actor()
        {
            var actor = Sys.ActorOf(Props.Create(() => new TestLogActor()), "testLoggingActor");
            await actor.Ask<TestMessage>(new TestMessage("ping"));
            await Task.Delay(500);
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
        public async Task ShouldLog_from_test_system_actor()
        {
            var actor = Sys.ActorOf(Props.Create(() => new TestLogActor()));
            await actor.Ask<TestMessage>(new TestMessage("ping"));
            await Task.Delay(500);
        }
    }
}