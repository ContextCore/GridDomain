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
        public FutureEventsTest_Persistent(): base(new AutoTestAkkaConfiguration().ToStandAloneSystemConfig(), "testSystem", true)
        {
        }

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(5);

        protected override IQuartzConfig CreateQuartzConfig()
        {
            return new PersistedQuartzConfig();
        }
        [Test]
        public void Given_aggregate_When_raising_future_event_Then_it_fires_in_time()
        {
            var scheduledTime = DateTime.Now.AddSeconds(1);
            var aggregate = RaiseFutureEventInTime(scheduledTime);
            Assert.LessOrEqual(scheduledTime.Second - aggregate.ProcessedTime.Second, 1);
        }


        [Test]
        public void Given_aggregate_When_raising_future_event_in_past_Then_it_fires_immediatly()
        {
            var scheduledTime = DateTime.Now.AddSeconds(-10);
            var now = DateTime.Now;
            var aggregate = RaiseFutureEventInTime(scheduledTime);
            Assert.LessOrEqual(now.Second - aggregate.ProcessedTime.Second, 1);
        }
    }
}