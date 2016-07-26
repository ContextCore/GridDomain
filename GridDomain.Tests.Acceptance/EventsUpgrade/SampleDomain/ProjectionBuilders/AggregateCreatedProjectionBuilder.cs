using System.Collections.Generic;
using System.Diagnostics;
using GridDomain.CQRS;
using GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain.Events;

namespace GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain.ProjectionBuilders
{
    public class AggregateCreatedProjectionBuilder : IHandler<AggregateCreatedEvent>
    {
        private static Stopwatch watch = new Stopwatch();
        static AggregateCreatedProjectionBuilder()
        {
            watch.Start();
        }
        public AggregateCreatedProjectionBuilder(IList<object> projectedEvents)
        {
            _projectedEvents = projectedEvents;
        }

        private int number = 0;
        private readonly IList<object> _projectedEvents;
        public static int ProjectionGroupHashCode { get; set; }
        public void Handle(AggregateCreatedEvent msg)
        {
            _projectedEvents.Add(msg);
        }
    }
}