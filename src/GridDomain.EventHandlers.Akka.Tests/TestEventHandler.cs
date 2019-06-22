using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GridDomain.EventHandlers.Akka.Tests
{
    public class TestEventHandler : IEventHandler<TestMessage>
    {
        public List<TestMessage> Messages { get; } = new List<TestMessage>();
        public Task Handle(Sequenced<TestMessage>[] evt)
        {
            Messages.AddRange(evt.Select(e => e.Message));
            return Task.CompletedTask;
        }
    }
}