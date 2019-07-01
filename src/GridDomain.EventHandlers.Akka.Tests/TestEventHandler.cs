using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GridDomain.EventHandlers.Akka.Tests
{
    public class TestEventHandler : IEventHandler<TestMessage>, IEventHandler<AnotherTestMessage>
    {
        public List<TestMessage> Messages { get; } = new List<TestMessage>();
        public List<AnotherTestMessage> AnotherMessages { get; } = new List<AnotherTestMessage>();
        public Task Handle(Sequenced<TestMessage>[] evt)
        {
            Messages.AddRange(evt.Select(e => e.Message));
            return Task.Delay(100);
        }

        public Task Handle(Sequenced<AnotherTestMessage>[] evt)
        {
            AnotherMessages.AddRange(evt.Select(e => e.Message));
            return Task.Delay(100);
        }
    }
}