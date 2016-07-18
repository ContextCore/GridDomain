using System.Diagnostics;
using GridDomain.CQRS;
using GridDomain.Tests.SampleDomain.Events;

namespace GridDomain.Tests.SampleDomain.ProjectionBuilders
{
    public class AggregateCreatedProjectionBuilder_Alternative : IHandler<AggregateCreatedEvent>
    {
        private static Stopwatch watch = new Stopwatch();
        static AggregateCreatedProjectionBuilder_Alternative()
        {
            watch.Start();
        }

        private int number = 0;
        public static int ProjectionGroupHashCode { get; set; }

        public void Handle(AggregateCreatedEvent msg)
        {
            msg.History.ProjectionGroupHashCode = ProjectionGroupHashCode;
            msg.History.SequenceNumber = ++number;
            msg.History.ElapsedTicksFromAppStart = watch.ElapsedTicks;
        }
    }
}