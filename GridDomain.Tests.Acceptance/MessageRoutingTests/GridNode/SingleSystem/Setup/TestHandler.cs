using Akka.Actor;
using GridDomain.CQRS;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode.SingleSystem.Setup
{
    public class TestHandler : IHandler<TestMessage>
    {
        private readonly IActorRef _notifier;
        private int _handleCounter = 0;

        public TestHandler(IActorRef notifier)
        {
            _notifier = notifier;
        }

        public void Handle(TestMessage e)
        {
            e.HandlerHashCode = GetHashCode();
            e.HandleOrder = ++_handleCounter;
            _notifier.Tell(e);
        }
    }
}