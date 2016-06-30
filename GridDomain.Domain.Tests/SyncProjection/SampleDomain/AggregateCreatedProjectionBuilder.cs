using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.SyncProjection.SampleDomain
{
    public class AggregateCreatedProjectionBuilder : IHandler<AggregateCreatedEvent>
    {
        private int number = 0;
        public void Handle(AggregateCreatedEvent msg)
        {
            msg.History.ProcessorHashCode = this.GetHashCode();
            msg.History.SequenceNumber = ++number;
            msg.History.Time = DateTime.Now;
        }
    }
}