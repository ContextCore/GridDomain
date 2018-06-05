using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.FutureEvents
{
    public class Cluster_Raising_future_events_too_late : Raising_future_events_too_late
    {
        public Cluster_Raising_future_events_too_late(ITestOutputHelper output) :
            base(new FutureEventsFixture(output).Clustered()) {}
    }
}