using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Actors.CommandPipe
{
    public class HandlersExecuted 
    {
        public DomainEvent[] DomainEvents { get; }
        public IMessageMetadata Metadata { get; }
        public IFault Fault { get; }

        public HandlersExecuted(IMessageMetadata metadata, DomainEvent[]  events)
        {
            DomainEvents = events;
            Metadata = metadata;
        }
        public HandlersExecuted(IMessageMetadata metadata, IFault fault)
        {
            Fault = fault;
            Metadata = metadata;
            DomainEvents = new  DomainEvent[] {};
        }
    }
}