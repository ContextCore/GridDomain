using System.Threading.Tasks;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using GridDomain.Node.Configuration;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Logging {
    /// <summary>
    /// 
    /// </summary>
    public class LogForCustomizedSystemTests : TestKit
    {

        private static string GetConfig()
        {
            return ActorSystemConfigBuilder.New()
                                     .Log(LogEventLevel.Verbose,null,true)
                                     .DomainSerialization()
                                     .RemoteActorProvider()
                                     .Remote(new NodeNetworkAddress())
                                     .InMemoryPersistence()
                                     .Build();
        }
        public LogForCustomizedSystemTests(ITestOutputHelper output):base(GetConfig())
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

            Sys.Log.Info("Hi from system info, payload : {@p}", new TestMessage("ping"));
            Sys.Log.Warning("Hi from system warn, payload : {@p}", new TestMessage("ping"));
            Sys.Log.Debug("Hi from system debug, payload : {@p}", new TestMessage("ping"));
            Sys.Log.Error("Hi from system error, payload : {@p}", new TestMessage("ping"));

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
    }
}