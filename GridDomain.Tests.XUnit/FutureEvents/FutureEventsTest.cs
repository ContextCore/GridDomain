using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.FutureEvents.Infrastructure;
using Microsoft.Practices.Unity;
using Quartz;
using Serilog.Events;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.FutureEvents
{
    internal class FutureEventsFixture : NodeTestFixture
    {
        public IScheduler Scheduler;

        protected override void OnNodeStarted()
        {
            Scheduler = Node.Container.Resolve<IScheduler>();
            Scheduler.Clear();
        }

        protected override IContainerConfiguration CreateContainerConfiguration()
        {
            return new CustomContainerConfiguration(c => c.RegisterAggregate<TestAggregate, TestAggregatesCommandHandler>());
        }

        protected override IMessageRouteMap CreateRouteMap()
        {
            return new TestRouteMap();
        }
    }

    public class FutureEventsTest : NodeTestKit
    {
        public FutureEventsTest(ITestOutputHelper output) : base(output, new FutureEventsFixture()) {}
    }
}