using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.FutureEvents
{
    public class Cluster_Reraising_future_event : Reraising_future_event
    {
        public Cluster_Reraising_future_event(ITestOutputHelper output) 
           : base(new FutureEventsFixture(output).Clustered()) {}
    }
}