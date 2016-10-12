using System;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing.Sagas.FutureEvents
{
    public class AsyncEventsReceived
    {

        public ICommand Command { get; }
        public DomainEvent[] ProducedEvents { get; }
        public Guid InvocationId { get; }

        public Exception Exception { get; }
        public AsyncEventsReceived(DomainEvent[] producedEvents, ICommand command, Guid invocationId, Exception ex = null)
        {
            Command = command;
            ProducedEvents = producedEvents;
            this.InvocationId = invocationId;
            Exception = ex;
        }

    }
}