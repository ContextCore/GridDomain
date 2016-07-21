using GridDomain.Tests.FutureEvents;
using GridDomain.Tests.FutureEvents.Cancelation;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.FutureDomainEvents
{
    [TestFixture]
    public class Given_future_event_in_aggregate_When_cancelling_it_Persistent : Given_future_event_in_aggregate_When_cancelling_it
    {
        public Given_future_event_in_aggregate_When_cancelling_it_Persistent():base(false)
        {
            
        }   
    }
}