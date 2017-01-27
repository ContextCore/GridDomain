using System;
using System.Linq;
using System.Threading;
using GridDomain.Tests.XUnit.SampleDomain;
using Xunit;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
  
    public class When_aggregate_raise_async_events
    {
       [Fact]
        public void It_places_continuation_in_uncommited_async_events_collection()
        {
            var aggregate = WhenRaiseAsyncEvents();
            Assert.Equal(1, aggregate.GetAsyncUncomittedEvents().Count);
        }

       [Fact]
        public void Nothing_is_applied_to_aggregate_on_async_finish()
        {
            var aggregate = WhenRaiseAsyncEvents();
            Thread.Sleep(2000);
            Assert.Null(aggregate.Value);
        }

        private static SampleAggregate WhenRaiseAsyncEvents()
        {
            var aggregate = new SampleAggregate(Guid.NewGuid(), null);
            aggregate.ChangeStateAsync(42,TimeSpan.FromMilliseconds(500));
            return aggregate;
        }


       [Fact]
        public void Then_it_results_can_be_applied_to_aggregate()
        {
            var aggregate = WhenRaiseAsyncEvents();
            var asyncEvents = aggregate.GetAsyncUncomittedEvents().First();
            Thread.Sleep(1500);
            aggregate.FinishAsyncExecution(asyncEvents.InvocationId);
            Assert.Equal("42", aggregate.Value);
        }

    }
}