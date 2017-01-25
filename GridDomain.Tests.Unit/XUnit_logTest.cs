using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using GridDomain.Tests.Unit.CommandsExecution;
using NMoneys;
using NUnit.Framework;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit
{
    public class XUnit_logTest : SampleDomainCommandExecutionTests
    {

        class TestLogActor : ReceiveActor

        {
            private readonly ILoggingAdapter _log = Context.GetLogger();

            public TestLogActor(ITestOutputHelper helper)
            {
                helper.WriteLine("Hi from xunit");
                _log.Debug("actor created debug");
                _log.Info("actor info");
                _log.Error("actor error");
                _log.Warning("actor warn");

                Console.WriteLine("Hi from console");

                ReceiveAny(o =>
                {
                    Console.WriteLine("Hi from console on receive");
                    helper.WriteLine("Hi from xunit on message receive");
                    _log.Debug("Debug: received " + o);
                    _log.Info("Info: received " + o);
                    _log.Error("Error: received " + o);
                    _log.Warning("Warning: received " + o);
                    Sender.Tell(o);
                });
            }

        }

        private readonly ITestOutputHelper _helper;

        public XUnit_logTest(ITestOutputHelper  helper)
        {
            _helper = helper;
            Start();
        }

        [Fact]
        public async Task ShouldLog_from_gridNode_actor()
        {
            var actor = GridNode.System.ActorOf(Props.Create(() => new TestLogActor(_helper)), "testLoggingActor");
            await actor.Ask<string>("ping");
            Thread.Sleep(500);
        }

        [Fact]
        public async Task ShouldLog_from_test_system_actor()
        {
            var actor = Sys.ActorOf(Props.Create(() => new TestLogActor(_helper)));
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