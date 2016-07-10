using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Node.FutureEvents
{
    public class AsyncMethodCompleted
    {
        public ICommand Command { get; }
        public DomainEvent[] ProducedEvents { get; }

        public AsyncMethodCompleted(DomainEvent[] producedEvents, ICommand command)
        {
            Command = command;
            ProducedEvents = producedEvents;
        }
    }
}