using System;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Node.FutureEvents
{
    public class AsyncEventsRecieved
    {

        public ICommand Command { get; }
        public DomainEvent[] ProducedEvents { get; }
        public Guid InvocationId { get; }
        public AsyncEventsRecieved(DomainEvent[] producedEvents, ICommand command, Guid invocationId)
        {
            Command = command;
            ProducedEvents = producedEvents;
            this.InvocationId = invocationId;
        }

    }
}