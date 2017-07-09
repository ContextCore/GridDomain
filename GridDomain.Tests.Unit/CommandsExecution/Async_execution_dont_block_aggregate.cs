using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{


    public class Async_emit_after_await_results_to_the_new_state : BalloonDomainCommandExecutionTests
    {
        public Async_emit_after_await_results_to_the_new_state(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task When_aggregate_await_async_emit_produced_events_applied_to_state()
        {
            var asyncCommand = new DoubleIncreaseTitleCommand(43, Guid.NewGuid());
            var received = new HashSet<Guid>();
            var waiter = Node.NewWaiter().Expect<BalloonTitleChanged>(e =>
                                                                      {
                                                                          received.Add(e.Id);
                                                                          return received.Count == 2;
                                                                      }).Create();
            Node.Execute(asyncCommand);

            var res = waiter.Result;


            var event_generated_from_changed_state = res.Message<BalloonTitleChanged>();

            var finalValue = (asyncCommand.Value + 1).ToString();

            Assert.Equal(finalValue, event_generated_from_changed_state.Value);

            var sampleAggregate = await this.LoadAggregateByActor<Balloon>(asyncCommand.AggregateId);
            Assert.Equal(finalValue, sampleAggregate.Title);
        }
    }

    public class Async_execution_dont_block_aggregate : BalloonDomainCommandExecutionTests
    {
        public Async_execution_dont_block_aggregate(ITestOutputHelper output) : base(output) {}

        [Fact]
        public async Task When_async_method_is_called_other_commands_can_be_executed_before_async_results()
        {
            var aggregateId = Guid.NewGuid();
            var asyncCommand = new PlanTitleChangeCommand(43, Guid.NewGuid(), Guid.NewGuid(), TimeSpan.FromSeconds(1));
            var syncCommand = new WriteTitleCommand(42, aggregateId);

            var asyncCommandTask = Node.Prepare(asyncCommand)
                                       .Expect<BalloonTitleChanged>()
                                       .Execute();

            await Node.Prepare(syncCommand)
                      .Expect<BalloonTitleChanged>()
                      .Execute();

            var sampleAggregate = await this.LoadAggregateByActor<Balloon>(syncCommand.AggregateId);

            Assert.Equal(syncCommand.Parameter.ToString(), sampleAggregate.Title);

            var changedEvent = (await asyncCommandTask).Message<BalloonTitleChanged>();
            Assert.Equal(asyncCommand.Parameter.ToString(), changedEvent.Value);
        }
    }
}