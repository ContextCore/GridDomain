using Akka.Actor;
using Akka.Event;
using GridDomain.Logging;
using GridDomain.Tests.Unit.CommandsExecution;
using NUnit.Framework;

namespace GridDomain.Tests.Unit
{
    [TestFixture]
    public class LogTest : SampleDomainCommandExecutionTests
    {
        [Test]
        public void ShouldLogFrom_method()
        {
            var logger = LogManager.GetLogger();

            logger.Info("test info");
            logger.Debug("test debug");
            logger.Trace("test trace");
        }

        class TestLogActor : ReceiveActor
        {

            public TestLogActor()
            {
                var log = Context.GetLogger();
                log.Debug("actor created debug");
                log.Info("actor info");
                log.Error("actor error");
                log.Warning("actor warn");
            }
        }

        [Test]
        public void ShouldLog_from_actor()
        {
            var actor = GridNode.System.ActorOf(Props.Create(() => new TestLogActor()));
        }
    }
}