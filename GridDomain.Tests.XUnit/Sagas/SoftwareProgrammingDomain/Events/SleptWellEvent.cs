using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events
{
    public class SleptWellEvent : DomainEvent
    {
        public SleptWellEvent(Guid sourceId, Guid sofaId, Guid sagaId = new Guid(), DateTime? createdTime = null)
            : base(sourceId, createdTime, sagaId)
        {
            SofaId = sofaId;
        }

        public Guid SofaId { get; }
        public Guid PersonId => SourceId;
    }
}