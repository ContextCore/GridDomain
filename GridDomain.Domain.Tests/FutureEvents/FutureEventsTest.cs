using System;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Framework;
using GridDomain.Tests.FutureEvents.Infrastructure;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.FutureEvents
{
    public abstract class FutureEventsTest : NodeCommandsTest
    {
        protected FutureEventsTest(string config, string name = null, bool clearDataOnStart = true) : base(config, name, clearDataOnStart)
        {
        }

        protected abstract IQuartzConfig CreateQuartzConfig();
        protected override GridDomainNode CreateGridDomainNode(AkkaConfiguration akkaConf,
            IDbConfiguration dbConfig)
        {
            var config = new CustomContainerConfiguration(
                c => c.RegisterAggregate<TestAggregate, TestAggregatesCommandHandler>(),
                c => c.RegisterInstance(CreateQuartzConfig()));

            var node =new GridDomainNode(config, new TestRouteMap(), TransportMode.Standalone, Sys);
            return node;
        }

        protected TestAggregate RaiseFutureEventInTime(DateTime scheduledTime)
        {
            var testCommand = new RaiseEventInFutureCommand(scheduledTime, Guid.NewGuid(), "test value");

            ExecuteAndWaitFor<TestDomainEvent>(testCommand);

            return LoadAggregate<TestAggregate>(testCommand.AggregateId);
        }
    }
}