using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Acceptance.XUnit.EventsUpgrade.SampleDomain
{
    internal class EventB : DomainEvent
    {
        public EventB(Guid sourceId, IOrder order) : base(sourceId)
        {
            Order = order;
        }

        public IOrder Order { get; }
    }
}