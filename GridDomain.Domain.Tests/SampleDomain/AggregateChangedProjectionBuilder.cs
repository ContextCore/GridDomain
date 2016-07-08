using System;
using System.Diagnostics;
using GridDomain.CQRS;

namespace GridDomain.Tests.SyncProjection.SampleDomain
{



    public class AggregateChangedProjectionBuilder : IHandler<AggregateChangedEvent>
    {
        public static int ProjectionGroupHashCode;
        private static Stopwatch watch = new Stopwatch();
        static AggregateChangedProjectionBuilder()
        {
            watch.Start();
        }
        private int number = 0;
        public void Handle(AggregateChangedEvent msg)
        {
            msg.History.ProjectionGroupHashCode = ProjectionGroupHashCode;
            msg.History.SequenceNumber = ++number;
            msg.History.ElapsedTicksFromAppStart = watch.ElapsedTicks;
        }
    }
}