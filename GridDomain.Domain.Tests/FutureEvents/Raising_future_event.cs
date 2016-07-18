using System;
using NUnit.Framework;

namespace GridDomain.Tests.FutureEvents
{
    [TestFixture]

    public class Raising_future_event : FutureEventsTest_InMemory
    {
        [Test]
        public void Given_aggregate_When_raising_future_event_Then_it_fires_in_time()
        {
            var scheduledTime = DateTime.Now.AddSeconds(1);
            var aggregate = RaiseFutureEventInTime(scheduledTime);
            Assert.LessOrEqual(scheduledTime.Second - aggregate.ProcessedTime.Second,1);
        }
    }
}