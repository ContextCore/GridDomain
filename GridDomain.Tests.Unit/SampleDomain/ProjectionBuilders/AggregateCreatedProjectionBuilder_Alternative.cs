using System.Diagnostics;
using GridDomain.CQRS;
using GridDomain.Tests.Unit.SampleDomain.Events;

namespace GridDomain.Tests.Unit.SampleDomain.ProjectionBuilders
{
    public class AggregateCreatedProjectionBuilder_Alternative : IHandler<SampleAggregateCreatedEvent>
    {
        private static Stopwatch watch = new Stopwatch();
        static AggregateCreatedProjectionBuilder_Alternative()
        {
            watch.Start();
        }

        private int number = 0;
        public static int ProjectionGroupHashCode { get; set; }

        public void Handle(SampleAggregateCreatedEvent msg)
        {
            msg.History.ProjectionGroupHashCode = ProjectionGroupHashCode;
            msg.History.SequenceNumber = ++number;
            msg.History.ElapsedTicksFromAppStart = watch.ElapsedTicks;
            msg.History.HandlerName = this.GetType().Name;
        }
    }
}