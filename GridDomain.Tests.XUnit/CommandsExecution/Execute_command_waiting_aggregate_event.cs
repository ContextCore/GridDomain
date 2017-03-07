using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.XUnit.SampleDomain.Commands;
using GridDomain.Tests.XUnit.SampleDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
    public class Execute_command_waiting_aggregate_event : SampleDomainCommandExecutionTests
    {
        public Execute_command_waiting_aggregate_event(ITestOutputHelper output) : base(output) {}

        [Fact]
        public async Task After_wait_ends_aggregate_should_be_changed()
        {
            var cmd = new LongOperationCommand(100, Guid.NewGuid());

            var res = await Node.Prepare(cmd)
                                .Expect<SampleAggregateChangedEvent>(e => e.SourceId == cmd.AggregateId)
                                .Execute();

            var msg = res.Message<SampleAggregateChangedEvent>();

            Assert.Equal(cmd.Parameter.ToString(), msg.Value);
        }

        [Fact]
        public async Task After_wait_of_async_command_aggregate_should_be_changed()
        {
            var cmd = new AsyncMethodCommand(42, Guid.NewGuid(), Guid.NewGuid(), TimeSpan.FromMilliseconds(50));
            var res = await Node.Prepare(cmd)
                                .Expect<SampleAggregateChangedEvent>(e => e.SourceId == cmd.AggregateId)
                                .Execute();

            Assert.Equal(cmd.Parameter.ToString(), res.Message<SampleAggregateChangedEvent>().Value);
        }

        [Fact]
        public async Task CommandWaiter_Should_wait_until_aggregate_event()
        {
            var cmd = new LongOperationCommand(100, Guid.NewGuid());

            var res = await Node.Prepare(cmd)
                                .Expect<SampleAggregateChangedEvent>(e => e.SourceId == cmd.AggregateId)
                                .Execute();

            var msg = res.Message<SampleAggregateChangedEvent>();

            Assert.Equal(cmd.Parameter.ToString(), msg.Value);
        }

        [Fact]
        public async Task CommandWaiter_will_wait_for_all_of_expected_message()
        {
            var cmd = new LongOperationCommand(1000, Guid.NewGuid());

            await Node.Prepare(cmd)
                      .Expect<SampleAggregateChangedEvent>(e => e.SourceId == cmd.AggregateId)
                      .And<SampleAggregateCreatedEvent>(e => e.SourceId == cmd.AggregateId)
                      .Execute()
                      .ShouldThrow<TimeoutException>();
        }

        [Fact]
        public async Task MessageWaiter_after_cmd_execute_should_waits_until_aggregate_event()
        {
            var cmd = new LongOperationCommand(100, Guid.NewGuid());
            var waiter = Node.NewExplicitWaiter()
                             .Expect<IMessageMetadataEnvelop<SampleAggregateChangedEvent>>(e => e.Message.SourceId == cmd.AggregateId)
                             .Create();

            Node.Execute(cmd);

            var res = await waiter;

            Assert.Equal(cmd.Parameter.ToString(), res.Message<SampleAggregateChangedEvent>().Value);
        }

        [Fact]
        public async Task Wait_for_timeout_command_throws_excpetion()
        {
            var cmd = new LongOperationCommand(1000, Guid.NewGuid());
            await Node.Prepare(cmd)
                      .Expect<SampleAggregateChangedEvent>(e => e.SourceId == cmd.AggregateId)
                      .Execute(TimeSpan.FromMilliseconds(100))
                      .ShouldThrow<TimeoutException>();
        }
    }
}