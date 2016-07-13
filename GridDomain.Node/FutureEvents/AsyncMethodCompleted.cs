using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Node.FutureEvents
{
    public class AsyncEventsRecieved
    {

        public ICommand Command { get; }
        public DomainEvent[] ProducedEvents { get; }
        public Guid InvocationId { get; }

        public Exception Exception { get; }
        public AsyncEventsRecieved(DomainEvent[] producedEvents, ICommand command, Guid invocationId, Exception ex = null)
        {
            Command = command;
            ProducedEvents = producedEvents;
            this.InvocationId = invocationId;
            Exception = ex;
        }

    }
}