using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events
{
    public class CoffeMakeFailedEvent : DomainEvent
    {
        public CoffeMakeFailedEvent(Guid sourceId, Guid forPersonId, DateTime? createdTime = null, Guid sagaId = new Guid())
            : base(sourceId,  sagaId, createdTime: createdTime)
        {
            ForPersonId = forPersonId;
        }

        public Guid CoffeMachineId => SourceId;
        public Guid ForPersonId { get; }
    }
}