using System.Linq;
using GridDomain.Domain.Tests;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests
{
    [TestFixture]
    public class Given_not_correlated_routing : RoutingTests
    {
        private TestMessage[] _initialCommands;
        private TestMessage[] _resultMessages;
        private long _handlerId;
        private int _threadId;

        [SetUp]
        public void Given_correlated_routing_for_message()
        {
            Router.Route<TestMessage>()
                .To<TestHandler>()
                .Register();
            Router.WaitForRouteConfiguration();

            _initialCommands = When_publishing_messages_with_same_correlation_id();
            _resultMessages = WaitFor(_initialCommands.Length);

            _handlerId = _resultMessages.First().HandlerHashCode;
        }

        [Test]
        public void Then_results_should_be_from_initial_commands()
        {
            CollectionAssert.AreEquivalent(_initialCommands.Select(c => c.Id), _resultMessages.Select(r => r.Id));
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

    }
}