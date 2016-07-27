using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events
{
    public class SleptWellEvent: DomainEvent
    {
        public SleptWellEvent(Guid sourceId, DateTime? createdTime = null, Guid sagaId = new Guid()) : base(sourceId, createdTime, sagaId)
        {
        }
    }
}