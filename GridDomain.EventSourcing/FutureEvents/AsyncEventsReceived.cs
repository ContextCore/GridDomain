using System;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing.FutureEvents
{
    public class AsyncEventsReceived
    {
        public AsyncEventsReceived(DomainEvent[] producedEvents, ICommand command, Guid invocationId, Exception ex = null)
        {
            Command = command;
            ProducedEvents = producedEvents;
            InvocationId = invocationId;
            Exception = ex;
        }

        public ICommand Command { get; }
        public DomainEvent[] ProducedEvents { get; }
        public Guid InvocationId { get; }

        public Exception Exception { get; }
    }
}