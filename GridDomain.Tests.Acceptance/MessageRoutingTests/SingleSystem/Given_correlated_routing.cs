using System.Linq;
using GridDomain.Domain.Tests;
using GridDomain.Node.AkkaMessaging;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests
{
    [TestFixture]

    public class Given_correlated_routing : SingleActorSystemTest
    {
        protected override void ConfigureRouter(ActorMessagesRouter router)
        {
            router.Route<TestMessage>()
                  .To<TestHandler>()
                  .WithCorrelation(nameof(TestMessage.CorrelationId))
                  .Register();

            router.WaitForRouteConfiguration();
        }

        [Test]
        public void Then_results_should_be_from_initial_commands()
        {
            CollectionAssert.AreEquivalent(_initialCommands.Select(c => c.Id), _resultMessages.Select(r => r.Id));
        }

        [Then]
        public void Then_It_should_be_handled_in_execute_sequence()
        {
            Assert.True(_resultMessages.All(m => m.HandleOrder == m.ExecuteOrder));
        }

        [Then]
        public void Then_It_should_be_handled_in_single_handler_instance()
        {
            foreach (var correlationGroup in _resultMessages.GroupBy(m => m.CorrelationId))
            {
                var handlerHashCode = correlationGroup.First().HandlerHashCode;
                Assert.True(correlationGroup.All(m => m.HandlerHashCode == handlerHashCode));
            }
        }

    }
}
