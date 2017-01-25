using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.FutureEvents
{
    [TestFixture]
    public class Raising_future_events_too_late : FutureEventsTest
    {
        public Raising_future_events_too_late(bool inMemory) : base(inMemory)
        {
            
        }
        public Raising_future_events_too_late() : base(true)
        {

        }


        [Test]
        public async Task Given_aggregate_When_raising_future_event_in_past_Then_it_fires_immediatly()
        {
            var scheduledTime = DateTime.Now.AddSeconds(-5);
            var now = DateTime.Now;
            var aggregate = await RaiseFutureEventInTime(scheduledTime);
            Assert.LessOrEqual(now.Second - aggregate.ProcessedTime.Second, 1);
        }

        protected override TimeSpan DefaultTimeout => TimeSpan.FromSeconds(3);
    }
}