using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.CQRS;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode.SingleSystem.Setup
{
    public class TestHandler : IHandler<TestEvent>
    {
        private readonly IActorRef _notifier;
        private int _handleCounter;

        public TestHandler(IActorRef notifier)
        {
            _notifier = notifier;
        }

        public Task Handle(TestEvent msg)
        {
            msg.HandlerHashCode = GetHashCode();
            msg.HandleOrder = ++_handleCounter;
            _notifier.Tell(msg);
            return Task.CompletedTask;
        }
    }
}