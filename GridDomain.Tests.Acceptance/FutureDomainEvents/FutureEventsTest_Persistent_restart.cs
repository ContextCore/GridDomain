using System;
using System.Threading;
using GridDomain.EventSourcing.Sagas.FutureEvents;
using GridDomain.Node;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
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

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(10);

        [OneTimeSetUp]
        public void Given_aggregate_When_raising_future_event()
        {
            _testCommand = new RaiseEventInFutureCommand(DateTime.Now.AddSeconds(5), 
                                                         Guid.NewGuid(), 
                                                         "test value");

            ExecuteAndWaitFor<FutureEventScheduledEvent>(_testCommand);
            //FutureEventScheduledEvent is a trigger for schedule an event to Quartz, lets give it some time to process
            Thread.Sleep(1000);
        }

        [Then]
        public void It_fires_after_node_restart()
        {
            GridNode.Stop();
            Thread.Sleep(1000);
            GridNode = CreateGridDomainNode(AkkaCfg, new LocalDbConfiguration());
            GridNode.Start(new LocalDbConfiguration());
          // event is not passed to waiter, but raised
            WaitFor<FutureEventOccuredEvent>();

            var aggregate = LoadAggregate<TestAggregate>(_testCommand.AggregateId);
            Assert.LessOrEqual(aggregate.ProcessedTime - _testCommand.RaiseTime, TimeSpan.FromSeconds(2));
        }
    }
}