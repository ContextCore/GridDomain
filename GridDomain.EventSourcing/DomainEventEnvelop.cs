using System;

namespace GridDomain.EventSourcing
{
    public class DomainEventEnvelop : IDomainEventEnvelop
    {
        public DomainEventEnvelop(ISourcedEvent @event,  IMetadata metadata)
        {
            Event = @event;
            Metadata = metadata;
        }

        public ISourcedEvent Event { get; }
        public IMetadata Metadata { get; }
        object IMessageEnvelop.Event => Event;
    }
}