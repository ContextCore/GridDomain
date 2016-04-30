using System;

namespace GridDomain.EventSourcing
{
    public class DomainEvent : IVersionedEvent
    {
        public DomainEvent(Guid sourceId, DateTime? createdTime = null)
        {
            SourceId = sourceId;
            CreatedTime = createdTime ?? DateTime.UtcNow;
        }

        //Source of the event - aggregate that created it
        public Guid SourceId { get; set; }
        public Guid SagaId { get; set; }
        public DateTime CreatedTime { get; set; }

        public virtual int Version { get; } = 1;
    }
}