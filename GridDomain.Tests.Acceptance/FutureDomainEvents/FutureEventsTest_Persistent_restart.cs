using System;
using System.Threading;
using GridDomain.Node;
using GridDomain.Node.Configuration.Composition;
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

            var testCommand = new TestCommand(scheduledTime, Guid.NewGuid());
            ExecuteAndWaitFor<FutureDomainEvent>(testCommand);

            Thread.Sleep(500); // to create scheduled task

            GridNode.Stop();

         //   Thread.Sleep(1000);

            var config = new CustomContainerConfiguration(
                c => c.RegisterAggregate<TestAggregate, TestAggregatesCommandHandler>(),
                c => c.RegisterInstance(CreateQuartzConfig()));

            var node = new GridDomainNode(config,
                                          new TestRouteMap(),
                                          TransportMode.Standalone,
                                          ActorSystemFactory.CreateActorSystem(new AutoTestAkkaConfiguration()));

            WaitFor<TestDomainEvent>();

            var aggregate = LoadAggregate<TestAggregate>(testCommand.AggregateId);
            Assert.AreEqual(scheduledTime.Second, aggregate.ProcessedTime.Second);
        }
    }
}