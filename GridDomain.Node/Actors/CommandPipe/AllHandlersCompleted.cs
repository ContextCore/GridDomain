using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Actors.CommandPipe
{
    public class AllHandlersCompleted 
    {
        public DomainEvent[] DomainEvents { get; }
        public IMessageMetadata Metadata { get; }
        public IFault Fault { get; }

        public AllHandlersCompleted(IMessageMetadata metadata, DomainEvent[]  events)
        {
            DomainEvents = events;
            Metadata = metadata;
        }
        public AllHandlersCompleted(IMessageMetadata metadata, IFault fault)
        {
            Fault = fault;
            Metadata = metadata;
            DomainEvents = new  DomainEvent[] {};
        }
    }
}