using System;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tests.FutureEvents;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.FutureDomainEvents
{
    [TestFixture]
    public class FutureEventsTest_Persistent : FutureEventsTest
    {
        public FutureEventsTest_Persistent(): base(new AutoTestAkkaConfiguration().ToStandAloneInMemorySystemConfig(), "testSystem", false)
        {
        }

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(2);

        protected override IQuartzConfig CreateQuartzConfig()
        {
            return new InMemoryQuartzConfig();
        }
        [Test]
        public void Given_aggregate_When_raising_future_event_Then_it_fires_in_time(DateTime timeToRaise)
        {
            var scheduledTime = DateTime.Now.AddSeconds(1);
            var aggregate = RaiseFutureEventInTime(scheduledTime);
            Assert.AreEqual(scheduledTime.Second, aggregate.ProcessedTime.Second);
        }

        [Test]
        public void Given_aggregate_When_raising_future_event_in_past_Then_it_fires_immediatly()
        {
            var scheduledTime = DateTime.Now.AddSeconds(-1);
            var now = DateTime.Now;
            var aggregate = RaiseFutureEventInTime(scheduledTime);
            Assert.AreEqual(now.Second, aggregate.ProcessedTime.Second);

        }
    }
}