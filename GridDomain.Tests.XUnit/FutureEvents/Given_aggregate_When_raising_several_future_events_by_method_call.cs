using System;
using CommonDomain;
using GridDomain.EventSourcing.FutureEvents;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.FutureEvents.Infrastructure;
using Xunit;

namespace GridDomain.Tests.XUnit.FutureEvents
{
    
    public class Given_aggregate_When_raising_several_future_events_by_method_call
    {

      [Fact]
        public void When_scheduling_future_event()
        {
            var aggregate = new TestAggregate(Guid.NewGuid());
            aggregate.ScheduleInFuture(DateTime.Now.AddSeconds(400),"value D");
            aggregate.ClearEvents();
      //Then_raising_event_with_wrong_id_throws_an_error()
            Assert.Throws<ScheduledEventNotFoundException>(() => aggregate.RaiseScheduledEvent(Guid.NewGuid(), Guid.NewGuid()));
        //Then_raising_event_with_wrong_id_does_not_produce_new_events()
            try
            {
                aggregate.RaiseScheduledEvent(Guid.NewGuid(), Guid.NewGuid());
            }
            catch
            {
                //intentionally left empty
            }
            Assert.Empty(aggregate.GetEvents());
        }
    }
}