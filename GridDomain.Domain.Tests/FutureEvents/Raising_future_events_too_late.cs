using System;
using NUnit.Framework;

namespace GridDomain.Tests.FutureEvents
{
    [TestFixture]
    public class Raising_future_events_too_late : FutureEventsTest_InMemory
    {


        [Test]
        public void Given_aggregate_When_raising_future_event_in_past_Then_it_fires_immediatly()
        {
            var scheduledTime = DateTime.Now.AddSeconds(-5);
            var now = DateTime.Now;
            var aggregate = RaiseFutureEventInTime(scheduledTime);
            Assert.LessOrEqual(now.Second - aggregate.ProcessedTime.Second, 1);
        }
    }
}