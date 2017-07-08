using System;
using System.Linq;
using GridDomain.EventSourcing;
using GridDomain.Scheduling.FutureEvents;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.DependencyInjection.FutureEvents.Infrastructure;
using Xunit;

namespace GridDomain.Tests.Unit.DependencyInjection.FutureEvents.Cancelation
{
    public class Given_aggregate_When_cancel_existing_future_event
    {
        [Fact]
        public void Then_it_occures_and_applies_to_aggregate()
        {
            var aggregate = new TestFutureEventsAggregate(Guid.NewGuid());
            aggregate.PersistAll();

            var testValue = "value D";

            aggregate.ScheduleInFuture(DateTime.Now.AddSeconds(400), testValue);

            var futureEventA = aggregate.GetEvent<FutureEventScheduledEvent>();
            aggregate.MarkPersisted(futureEventA);

            aggregate.ScheduleInFuture(DateTime.Now.AddSeconds(400), "test value E");

            var futureEventOutOfCriteria = aggregate.GetEvent<FutureEventScheduledEvent>();
            aggregate.MarkPersisted(futureEventOutOfCriteria);

            aggregate.CancelFutureEvents(testValue);

            // Cancelation_event_is_produced()
            var cancelEvent = aggregate.GetEvent<FutureEventCanceledEvent>();
            Assert.Equal(futureEventA.Id, cancelEvent.FutureEventId);
            aggregate.MarkPersisted(cancelEvent);
            //Only_predicate_satisfying_events_are_canceled()`
            var cancelEvents = aggregate.GetEvents<FutureEventCanceledEvent>();
            Assert.True(cancelEvents.All(e => e.FutureEventId != futureEventOutOfCriteria.Id));
            // Canceled_event_cannot_be_raised()

            Assert.Throws<ScheduledEventNotFoundException>(() => aggregate.RaiseScheduledEvent(futureEventA.Id, Guid.NewGuid()));

            var anyEvents = aggregate.GetEvents<DomainEvent>();
            Assert.Empty(anyEvents);
        }
    }
}