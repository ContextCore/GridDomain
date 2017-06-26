using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain
{
    internal class EventA : DomainEvent
    {
        public EventA(Guid sourceId, IOrder order) : base(sourceId)
        {
            Order = order;
        }

        public IOrder Order { get; }
    }
}