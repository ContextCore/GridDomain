using System.ServiceModel;
using GridDomain.Common;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Actors
{
    public class CustomHandlersProcessCompleted 
    {
        public DomainEvent[] DomainEvents { get; }
        public IMessageMetadata Metadata { get; }

        public CustomHandlersProcessCompleted(IMessageMetadata metadata, DomainEvent[]  events)
        {
            DomainEvents = events;
            Metadata = metadata;
        }
    }
}