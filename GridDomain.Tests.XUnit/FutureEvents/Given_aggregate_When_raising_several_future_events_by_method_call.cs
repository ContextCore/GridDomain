using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing.FutureEvents;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.CommandsExecution;
using GridDomain.Tests.XUnit.FutureEvents.Infrastructure;
using Xunit;

namespace GridDomain.Tests.XUnit.FutureEvents
{
    public class Given_aggregate_When_raising_several_future_events_by_method_call
    {
        [Fact]
        public async Task When_scheduling_future_event()
        {
            var aggregate = new FutureEventsAggregate(Guid.NewGuid());
            await aggregate.ScheduleInFuture(DateTime.Now.AddSeconds(400), "value D");

            aggregate.ClearEvents();
            //Then_raising_event_with_wrong_id_throws_an_error()
            
            await aggregate.RaiseScheduledEvent(Guid.NewGuid(), Guid.NewGuid())
                           .ShouldThrow<ScheduledEventNotFoundException>();

            //Then_raising_event_with_wrong_id_does_not_produce_new_events()
            try
            {
                await aggregate.RaiseScheduledEvent(Guid.NewGuid(), Guid.NewGuid());
            }
            catch
            {
                //intentionally left empty
            }
            Assert.Empty(aggregate.GetEvents());
        }
    }
}