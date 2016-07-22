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
        private RaiseEventInFutureCommand _testCommand;

        public FutureEventsTest_Persistent_restart(): base(false)
        {
        }

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(5);

        [TestFixtureSetUp]
        public void Given_aggregate_When_raising_future_event()
        {
            var scheduledTime = DateTime.Now.AddSeconds(5);
            _testCommand = new RaiseEventInFutureCommand(scheduledTime, Guid.NewGuid(), "test value");
            ExecuteAndWaitFor<FutureEventScheduledEvent>(_testCommand);
        }

        [Then]
        public void It_fires_after_node_restart()
        {
            GridNode.Stop();

        //    Thread.Sleep(2000); //to wait for everything stopped

            GridNode.Start(new LocalDbConfiguration());

            WaitFor<TestDomainEvent>();

            var aggregate = LoadAggregate<TestAggregate>(_testCommand.AggregateId);
            Assert.LessOrEqual(aggregate.ProcessedTime - _testCommand.RaiseTime, TimeSpan.FromSeconds(1));
        }
    }
}