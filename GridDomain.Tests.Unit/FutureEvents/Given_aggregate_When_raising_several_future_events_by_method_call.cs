using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.Scheduling;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;
using Xunit;

namespace GridDomain.Tests.Unit.FutureEvents
{
    public class Given_aggregate_When_raising_several_future_events_by_method_call
    {
        [Fact]
        public async Task When_scheduling_future_event()
        {
            var aggregate = new TestFutureEventsAggregate(Guid.NewGuid());
            aggregate.ScheduleInFuture(DateTime.Now.AddSeconds(400), "value D");
            aggregate.PersistAll();

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