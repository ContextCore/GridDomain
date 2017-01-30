using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.SampleDomain;
using GridDomain.Tests.XUnit.SampleDomain.Commands;
using GridDomain.Tests.XUnit.SampleDomain.Events;
using GridDomain.Tests.XUnit.SampleDomain.ProjectionBuilders;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
    public class CommandWaiter_waits_for_faults<TProcessException> : NodeTestKit, IClassFixture<SampleDomainContainerConfiguration>
    {

        public CommandWaiter_waits_for_faults(ITestOutputHelper helper, NodeTestFixture fixture):base(helper, fixture)
        {
        }

        [Fact]
        public async Task When_expected_fault_from_projection_group_call_received_it_contains_error()
        {
            var syncCommand = new LongOperationCommand(100, Guid.NewGuid());
            var res = await Node.Prepare(syncCommand)
                                .Expect<AggregateChangedEventNotification>(e => e.AggregateId == syncCommand.AggregateId)
                                .Or<IFault<SampleAggregateChangedEvent>>(f => f.Message.SourceId == syncCommand.AggregateId && 
                                                                                   f.Processor == typeof(OddFaultyMessageHandler))
                                .Execute(null,false);

            Assert.IsAssignableFrom<TProcessException>(res.Message<IFault<SampleAggregateChangedEvent>>()?.Exception);
        }

       [Fact]
        public async Task When_expected_fault_from_projection_group_task_received_it_contains_error()
        {
            var syncCommand = new LongOperationCommand(8, Guid.NewGuid());
            var res = await Node.Prepare(syncCommand)
                                    .Expect<AggregateChangedEventNotification>(e => e.AggregateId == syncCommand.AggregateId)
                                    .Or<IFault<SampleAggregateChangedEvent>>(f => f.Message.SourceId == syncCommand.AggregateId &&
                                                                                       f.Processor == typeof(OddFaultyMessageHandler))
                                    .Execute(null,false);

            Assert.IsAssignableFrom<TProcessException>(res.Message<IFault<SampleAggregateChangedEvent>>()?.Exception);
        }


       [Fact]
        public async Task When_expecting_generic_fault_without_processor_received_fault_contains_error()
        {
            var syncCommand = new LongOperationCommand(100, Guid.NewGuid());
            var res = await Node.Prepare(syncCommand)
                                    .Expect<AggregateChangedEventNotification>(e => e.AggregateId == syncCommand.AggregateId)
                                    .Or<IFault>(f => (f.Message as DomainEvent)?.SourceId == syncCommand.AggregateId)
                                    .Execute(false);

            Assert.IsAssignableFrom<TProcessException>(res.Message<IFault<SampleAggregateChangedEvent>>()?.Exception);
        }


       [Fact]
        public async Task When_does_not_expect_fault_and_it_accures_wait_times_out()
        {
            var syncCommand = new LongOperationCommand(100, Guid.NewGuid());
            await Node.Prepare(syncCommand)
                          .Expect<AggregateChangedEventNotification>(e => e.AggregateId == syncCommand.AggregateId)
                          .Execute(TimeSpan.FromMilliseconds(50))
                          .ShouldThrow<TimeoutException>();
        }


       [Fact]
        public async Task When_expected_optional_fault_does_not_occur_wait_is_successfull()
        {
            var syncCommand = new LongOperationCommand(101, Guid.NewGuid());
            var res = await Node.Prepare(syncCommand)
                                    .Expect<AggregateChangedEventNotification>(e => e.AggregateId == syncCommand.AggregateId)
                                    .Or<IFault>(f => (f.Message as DomainEvent)?.SourceId == syncCommand.AggregateId)
                                    .Execute(TimeSpan.FromSeconds(1000));

            var evt = res.Message<AggregateChangedEventNotification>();
            Assert.Equal(syncCommand.AggregateId, evt.AggregateId);
        }

       [Fact]
        public async Task When_fault_is_produced_when_publish_command_with_base_type()
        {
            var syncCommand = new AsyncFaultWithOneEventCommand(101, Guid.NewGuid());
            await Node.Prepare(syncCommand)
                          .Expect<AggregateChangedEventNotification>(e => e.AggregateId == syncCommand.AggregateId)
                          .Execute()
                          .ShouldThrow<SampleAggregateException>();
        }

       [Fact]
        public async Task When_aggregate_throws_fault_it_is_handled_without_implicit_registration()
        {
            //will throw exception in aggregate and in message handler
            var syncCommand = new AsyncFaultWithOneEventCommand(50, Guid.NewGuid());
            var res = await Node.Prepare(syncCommand)
                                .Expect<AggregateChangedEventNotification>()
                                .Or<IFault<SampleAggregateChangedEvent>>()
                                .Execute(false);

            Assert.NotNull(res.Message<IFault<AsyncFaultWithOneEventCommand>>());
        }


       [Fact]
        public async Task When_fault_was_received_and_failOnFaults_is_set_results_raised_an_error()
        {
            var syncCommand = new AsyncFaultWithOneEventCommand(100, Guid.NewGuid());
            await Node.Prepare(syncCommand)
                          .Expect<AggregateChangedEventNotification>()
                          .Execute()
                          .ShouldThrow<SampleAggregateException>();
        }
    }
}