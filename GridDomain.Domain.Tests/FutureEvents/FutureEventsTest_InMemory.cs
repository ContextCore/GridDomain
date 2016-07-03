using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Framework.Configuration;
using NUnit.Framework;

namespace GridDomain.Tests.FutureEvents
{
    [TestFixture]
    public class FutureEventsTest_InMemory : FutureEventsTest
    {
        public FutureEventsTest_InMemory(): base(new AutoTestAkkaConfiguration().ToStandAloneInMemorySystemConfig(), "testSystem", false)
        {
        }

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(2);

        protected override IQuartzConfig CreateQuartzConfig()
        {
            return new InMemoryQuartzConfig();
        }

        [Test]
        public void Given_aggregate_When_raising_future_event_Then_it_fires_in_time()
        {
            var scheduledTime = DateTime.Now.AddSeconds(1);
            var aggregate = RaiseFutureEventInTime(scheduledTime);
            Assert.AreEqual(scheduledTime.Second, aggregate.ProcessedTime.Second);
        }

        [Test]
        public void Given_aggregate_When_raising_future_event_in_past_Then_it_fires_immediatly()
        {
            var scheduledTime = DateTime.Now.AddSeconds(-5);
            var now = DateTime.Now;
            var aggregate = RaiseFutureEventInTime(scheduledTime);
            Assert.AreEqual(now.Second, aggregate.ProcessedTime.Second);

        }
    }
}
