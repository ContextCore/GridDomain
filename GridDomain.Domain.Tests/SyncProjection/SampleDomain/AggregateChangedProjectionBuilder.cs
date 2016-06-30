using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.SyncProjection.SampleDomain
{
    public class AggregateChangedProjectionBuilder : IHandler<AggregateChangedEvent>
    {
        private int number = 0;
        public void Handle(AggregateChangedEvent msg)
        {
            msg.History.ProcessorHashCode = this.GetHashCode();
            msg.History.SequenceNumber = ++number;
            msg.History.Time = DateTime.Now;
        }
    }
}