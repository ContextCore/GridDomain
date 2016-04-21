using System.Linq;
using Akka.Routing;
using GridDomain.Domain.Tests;
using GridDomain.Node.AkkaMessaging;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests
{
    [TestFixture]
    public class Single_system_not_correlated_routing : SingleActorSystemTest
    {
        private long _handlerId;

        [SetUp]
        public void Given_correlated_routing_for_message()
        {
            _handlerId = _resultMessages.First().HandlerHashCode;
        }

        [Test]
        public void Then_results_should_be_from_initial_commands()
        {
            CollectionAssert.AreEquivalent(InitialCommands.Select(c => c.Id), _resultMessages.Select(r => r.Id));
        }

        [Then]
        public void Then_It_should_be_handled_in_random_way()
        {
            Assert.True(_resultMessages.Any(m => m.HandleOrder != m.ExecuteOrder));
        }

        [Then]
        public void Then_It_should_be_handled_in_different_handler_instance()
        {
            Assert.True(_resultMessages.Any(m => m.HandlerHashCode != _handlerId));
        }

        protected override IRouterConfiguration CreateRoutes()
        {
            return new NotCorrelatedRouting<TestMessage, TestHandler>();
        }
    }
}