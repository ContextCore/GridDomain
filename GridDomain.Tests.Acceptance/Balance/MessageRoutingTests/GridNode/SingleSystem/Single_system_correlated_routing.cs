using System.Linq;
using GridDomain.Tests.Acceptance.Balance.MessageRoutingTests.GridNode.SingleSystem.Setup;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Balance.MessageRoutingTests.GridNode.SingleSystem
{
    [TestFixture]

    public class Single_system_correlated_routing : SingleActorSystemTest
    {
        [Test]
        public void Then_results_should_be_from_initial_commands()
        {
            CollectionAssert.AreEquivalent(InitialCommands.Select(c => c.Id), _resultMessages.Select(r => r.Id));
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

        protected override IRouterConfiguration CreateRoutes()
        {
            return new CorrelatedRouting<TestMessage,TestHandler>(nameof(TestMessage.CorrelationId));
        }
    }
}
