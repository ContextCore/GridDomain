using System;
using System.Threading;
using GridDomain.EventSourcing.Sagas.FutureEvents;
using GridDomain.Node;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Node.FutureEvents;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tests.FutureEvents;
using GridDomain.Tests.FutureEvents.Infrastructure;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.FutureDomainEvents
{
    [TestFixture]
    public class FutureEventsTest_Persistent_restart : FutureEventsTest
    {
        public FutureEventsTest_Persistent_restart(): base(new AutoTestAkkaConfiguration().ToStandAloneInMemorySystemConfig(), "testSystem", false)
        {
        }

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(5);

        protected override IQuartzConfig CreateQuartzConfig()
        {
            return new PersistedQuartzConfig();
        }

        [Test]
        public void Given_aggregate_When_raising_future_event_Then_it_fires_after_node_restart()
        {
            var scheduledTime = DateTime.Now.AddSeconds(5);

            var testCommand = new RaiseEventInFutureCommand(scheduledTime, Guid.NewGuid(), "test value");
            ExecuteAndWaitFor<FutureDomainEvent>(testCommand);

            Thread.Sleep(500); // to create scheduled task

            GridNode.Stop();

            Thread.Sleep(2000); //to wait for everything stopped

            GridNode.Start(new LocalDbConfiguration());

            WaitFor<TestDomainEvent>();

            var aggregate = LoadAggregate<TestAggregate>(testCommand.AggregateId);
            Assert.LessOrEqual(aggregate.ProcessedTime - scheduledTime, TimeSpan.FromSeconds(1));
        }
    }
}