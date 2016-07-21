using GridDomain.Tests.FutureEvents;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.FutureDomainEvents
{
    [TestFixture]
    public class Given_aggregate_When_raising_several_future_events_InMemory : Given_aggregate_When_raising_several_future_events
    {
        public Given_aggregate_When_raising_several_future_events_InMemory():base(false)
        {
            
        }
    }
}