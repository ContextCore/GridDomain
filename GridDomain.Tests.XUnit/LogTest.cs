using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Akka.Util;
using GridDomain.Logging;
using GridDomain.Tests.XUnit.CommandsExecution;
using NMoneys;
using Serilog;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit
{

    public class LogTest : SampleDomainCommandExecutionTests
    {

        class TestLogActor : ReceiveActor

        {
            private readonly ILoggingAdapter _log = Context.GetLogger();

            public TestLogActor()
            {
                _log.Debug("actor created debug");
                _log.Info("actor info");
                _log.Error("actor error");
                _log.Warning("actor warn");

                Console.WriteLine("Hi from console");

                ReceiveAny(o =>
                {
                    Console.WriteLine("Hi from console on receive");

                    _log.Debug("Debug: received " + o);
                    _log.Info("Info: received " + o);
                    _log.Error("Error: received " + o);
                    _log.Warning("Warning: received " + o);
                    Sender.Tell(o);
                });
            }

        }
        public LogTest(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldLog_from_gridNode_actor()
        {
            var actor = Node.System.ActorOf(Props.Create(() => new TestLogActor()),"testLoggingActor");
            await actor.Ask<string>("ping");
            Thread.Sleep(500);
        }

        [Fact]
        public async Task ShouldLog_from_test_system_actor()
        {
            var actor = Sys.ActorOf(Props.Create(() => new TestLogActor()));
            await actor.Ask<string>("ping");
            Thread.Sleep(500);
        }


        [Fact]
        public void Should_simplify_Money_class()
        {
            Serilog.Log.Logger.Error(new InvalidOperationException("ohshitwaddap"), "MONEY TEST {@placeholder}", new { Money = new Money(123, CurrencyIsoCode.RUB) });
        }
    }
}