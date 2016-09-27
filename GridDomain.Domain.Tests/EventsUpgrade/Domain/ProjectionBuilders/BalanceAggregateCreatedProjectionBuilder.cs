using System.Collections.Generic;
using System.Diagnostics;
using GridDomain.CQRS;
using GridDomain.Tests.EventsUpgrade.Domain.Events;

namespace GridDomain.Tests.EventsUpgrade.Domain.ProjectionBuilders
{
    public class BalanceAggregateCreatedProjectionBuilder : IHandler<AggregateCreatedEvent>
    {
        private static Stopwatch watch = new Stopwatch();
        static BalanceAggregateCreatedProjectionBuilder()
        {
            watch.Start();
        }
        public BalanceAggregateCreatedProjectionBuilder(IList<object> projectedEvents)
        {
            _projectedEvents = projectedEvents;
        }

        private readonly IList<object> _projectedEvents;
        public static int ProjectionGroupHashCode { get; set; }
        public void Handle(AggregateCreatedEvent msg)
        {
            _projectedEvents.Add(msg);
        }
    }
}