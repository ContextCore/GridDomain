using System;

namespace GridDomain.Tests.Unit.FutureEvents
{
    public class FutureEventsTest_InMemory : FutureEventsTest
    {
        public FutureEventsTest_InMemory() : base(true)
        {
        }

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(1);
    }
}
