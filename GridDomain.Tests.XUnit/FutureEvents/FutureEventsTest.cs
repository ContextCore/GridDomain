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
        public FutureEventsFixture()
        {
            Add(new CustomContainerConfiguration(c => c.RegisterAggregate<FutureEventsAggregate, FutureEventsAggregatesCommandHandler>()));
            Add(new FutureEventsRouteMap());
        }
        protected override void OnNodeStarted()
        {
            var scheduler = Node.Container.Resolve<IScheduler>();
            scheduler.Clear();
           
        }
    }

    public class FutureEventsTest : NodeTestKit
    {
        public FutureEventsTest(ITestOutputHelper output) : base(output, new FutureEventsFixture()) {}
    }
}