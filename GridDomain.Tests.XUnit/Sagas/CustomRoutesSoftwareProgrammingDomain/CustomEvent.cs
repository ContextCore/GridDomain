using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.XUnit.Sagas.CustomRoutesSoftwareProgrammingDomain
{
    public class CustomEvent : DomainEvent
    {
        public CustomEvent(Guid sourceId, DateTime? createdTime = null, Guid sagaId = new Guid())
            : base(sourceId, sagaId: sagaId, createdTime: createdTime) {}

        public string Payload { get; set; }
    }
}