using System.Diagnostics;
using GridDomain.CQRS;
using GridDomain.Tests.Unit.SampleDomain.Events;

namespace GridDomain.Tests.Unit.SampleDomain.ProjectionBuilders
{



    public class AggregateChangedProjectionBuilder : IHandler<SampleAggregateChangedEvent>
    {
        public static int ProjectionGroupHashCode;
        private static Stopwatch watch = new Stopwatch();
        static AggregateChangedProjectionBuilder()
        {
            watch.Start();
        }
        private int number = 0;
        public virtual void Handle(SampleAggregateChangedEvent msg)
        {
            msg.History.ProjectionGroupHashCode = ProjectionGroupHashCode;
            msg.History.SequenceNumber = ++number;
            msg.History.ElapsedTicksFromAppStart = watch.ElapsedTicks;
        }
    }
}