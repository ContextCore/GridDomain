using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.Scheduling;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;
using GridDomain.Tools;
using Xunit;

namespace GridDomain.Tests.Unit.FutureEvents.Cancelation
{
    public class Given_aggregate_When_cancel_existing_future_event
    {
        [Fact]
        public async Task Then_it_occures_and_applies_to_aggregate()
        {
            var aggregate = new TestFutureEventsAggregate(Guid.NewGuid());
            aggregate.CommitAll();

            var testValue = "value D";

            aggregate.ScheduleInFuture(DateTime.Now.AddSeconds(400), testValue);

            var futureEventA = aggregate.GetEvent<FutureEventScheduledEvent>();
            aggregate.Commit(futureEventA);

            aggregate.ScheduleInFuture(DateTime.Now.AddSeconds(400), "test value E");

            var futureEventOutOfCriteria = aggregate.GetEvent<FutureEventScheduledEvent>();
            aggregate.Commit(futureEventOutOfCriteria);

            aggregate.CancelFutureEvents(testValue);

            // Cancelation_event_is_produced()
            var cancelEvent = aggregate.GetEvent<FutureEventCanceledEvent>();
            Assert.Equal(futureEventA.Id, cancelEvent.FutureEventId);
            aggregate.Commit(cancelEvent);
            //Only_predicate_satisfying_events_are_canceled()`
            var cancelEvents = aggregate.GetEvents<FutureEventCanceledEvent>();
            Assert.True(cancelEvents.All(e => e.FutureEventId != futureEventOutOfCriteria.Id));
            // Canceled_event_cannot_be_raised()

            await aggregate.RaiseScheduledEvent(futureEventA.Id, Guid.NewGuid()).ShouldThrow<ScheduledEventNotFoundException>();

            var anyEvents = aggregate.GetEvents<DomainEvent>();
            Assert.Empty(anyEvents);
        }
    }
}