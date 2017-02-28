using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.XUnit.Sagas.CustomRoutesSoftwareProgrammingDomain
{
    public class CustomEvent : DomainEvent
    {
        public CustomEvent(Guid sourceId, DateTime? createdTime = null, Guid sagaId = new Guid())
            : base(sourceId, createdTime, sagaId) {}

        public string Payload { get; set; }
    }
}