using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain
{
    class EventB : DomainEvent
    {
        public IOrder Order { get; }

        public EventB(Guid sourceId, IOrder order) : base(sourceId)
        {
            Order = order;
        }
    }
}