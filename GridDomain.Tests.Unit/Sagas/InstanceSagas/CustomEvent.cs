using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.Sagas.InstanceSagas
{
    public class CustomEvent : DomainEvent
    {
        public string Payload { get; set; }

        public CustomEvent(Guid sourceId, DateTime? createdTime = null, Guid sagaId = new Guid()) : base(sourceId, createdTime, sagaId)
        {
        }
    }
}