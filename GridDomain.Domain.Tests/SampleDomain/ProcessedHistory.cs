using System;

namespace GridDomain.Tests.SyncProjection.SampleDomain
{
    public class ProcessedHistory
    {
        public int SequenceNumber;
        public long ElapsedTicksFromAppStart;
        public long ProjectionGroupHashCode;
    }
}