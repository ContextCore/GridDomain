using System;

namespace GridDomain.EventSourcing
{
    public class DomainEvent : IVersionedEvent
    {
        //Source of the event - aggregate that created it
        public Guid SourceId { get; set; }
        public Guid SagaId { get; set; }
        public DateTime CreatedTime { get; set; }

        public int Version { get; set; }
        public DomainEvent(Guid sourceId)
        {
            SourceId = sourceId;
        }
    }
}