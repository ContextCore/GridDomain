using System.ComponentModel;
using System.Linq;
using System.Net.Configuration;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Akka.Cluster.Tools.PublishSubscribe;
using GridDomain.Domain.Tests;
using GridDomain.Node.AkkaMessaging;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance
{

    [TestFixture]

    public class Given_correlated_routing : RoutingTests
    {
        private TestMessage[] _initialCommands;
        private TestMessage[] _resultMessages;

        [SetUp]
        public void Given_correlated_routing_for_message()
        {
            Router.Route<TestMessage>()
                   .To<TestHandler>()
                   .WithCorrelation(nameof(TestMessage.CorrelationId))
                   .Register();
            Router.WaitForRouteConfiguration();

            _initialCommands = When_publishing_messages_with_same_correlation_id();
            _resultMessages = WaitFor(_initialCommands.Length);

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
