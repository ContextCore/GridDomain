using System.Diagnostics;
using GridDomain.CQRS;

namespace GridDomain.Tests.SampleDomain
{
    public class AggregateCreatedProjectionBuilder : IHandler<AggregateCreatedEvent>
    {
        private static Stopwatch watch = new Stopwatch();
        static AggregateCreatedProjectionBuilder()
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