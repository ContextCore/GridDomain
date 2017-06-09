using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.FutureEvents;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.CommandsExecution;
using GridDomain.Tests.XUnit.FutureEvents.Infrastructure;
using Xunit;

namespace GridDomain.Tests.XUnit.FutureEvents.Cancelation
{
    public class Given_aggregate_When_cancel_not_existing_future_event
    {
        [Fact]
        public void Then_nothing_happened()
        {
            var aggregate = new FutureEventsAggregate(Guid.NewGuid());
            var testValue = "value D";

            aggregate.ScheduleInFuture(DateTime.Now.AddSeconds(400), testValue);

            aggregate.ClearEvents();

            aggregate.CancelFutureEvents("will not be found in any future event");
           //No_events_were_produced()
            Assert.Empty(aggregate.GetEvents<DomainEvent>());
        }
    }
}