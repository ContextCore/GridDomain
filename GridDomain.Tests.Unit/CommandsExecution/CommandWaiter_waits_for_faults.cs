using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Logging;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Unit.SampleDomain;
using GridDomain.Tests.Unit.SampleDomain.Commands;
using GridDomain.Tests.Unit.SampleDomain.Events;
using GridDomain.Tests.Unit.SampleDomain.ProjectionBuilders;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class CommandWaiter_waits_for_faults<TProcessException> : SampleDomainCommandExecutionTests
    {

        [Then]
        public async Task When_expected_fault_from_projection_group_call_received_it_contains_error()
        {
            var syncCommand = new LongOperationCommand(100, Guid.NewGuid());
            var res = await GridNode.PrepareCommand(syncCommand)
                                .Expect<AggregateChangedEventNotification>(e => e.AggregateId == syncCommand.AggregateId)
                                .Or<IFault<SampleAggregateChangedEvent>>(f => f.Message.SourceId == syncCommand.AggregateId && 
                                                                                   f.Processor == typeof(OddFaultyMessageHandler))
                                .Execute(null,false);

            Assert.IsInstanceOf<TProcessException>(res.Message<IFault<SampleAggregateChangedEvent>>()?.Exception);
        }

        [Then]
        public async Task When_expected_fault_from_projection_group_task_received_it_contains_error()
        {
            var syncCommand = new LongOperationCommand(8, Guid.NewGuid());
            var res = await GridNode.PrepareCommand(syncCommand)
                                    .Expect<AggregateChangedEventNotification>(e => e.AggregateId == syncCommand.AggregateId)
                                    .Or<IFault<SampleAggregateChangedEvent>>(f => f.Message.SourceId == syncCommand.AggregateId &&
                                                                                       f.Processor == typeof(OddFaultyMessageHandler))
                                    .Execute(null,false);

            Assert.IsInstanceOf<TProcessException>(res.Message<IFault<SampleAggregateChangedEvent>>()?.Exception);
        }


        [Then]
        public async Task When_expecting_generic_fault_without_processor_received_fault_contains_error()
        {
            var syncCommand = new LongOperationCommand(100, Guid.NewGuid());
            var res = await GridNode.PrepareCommand(syncCommand)
                                    .Expect<AggregateChangedEventNotification>(e => e.AggregateId == syncCommand.AggregateId)
                                    .Or<IFault>(f => (f.Message as DomainEvent)?.SourceId == syncCommand.AggregateId)
                                    .Execute(false);

            Assert.IsInstanceOf<TProcessException>(res.Message<IFault<SampleAggregateChangedEvent>>()?.Exception);
        }


        [Then]
        public async Task When_does_not_expect_fault_and_it_accures_wait_times_out()
        {
            var syncCommand = new LongOperationCommand(100, Guid.NewGuid());
            await GridNode.PrepareCommand(syncCommand)
                          .Expect<AggregateChangedEventNotification>(e => e.AggregateId == syncCommand.AggregateId)
                          .Execute(TimeSpan.FromMilliseconds(50))
                          .ShouldThrow<TimeoutException>();
        }


        [Then]
        public async Task When_expected_optional_fault_does_not_occur_wait_is_successfull()
        {
            var syncCommand = new LongOperationCommand(101, Guid.NewGuid());
            var res = await GridNode.PrepareCommand(syncCommand)
                                    .Expect<AggregateChangedEventNotification>(e => e.AggregateId == syncCommand.AggregateId)
                                    .Or<IFault>(f => (f.Message as DomainEvent)?.SourceId == syncCommand.AggregateId)
                                    .Execute(TimeSpan.FromSeconds(1000));

            var evt = res.Message<AggregateChangedEventNotification>();
            Assert.AreEqual(syncCommand.AggregateId, evt.AggregateId);
        }

        [Then]
        public async Task When_fault_is_produced_when_publish_command_with_base_type()
        {
            var syncCommand = new AsyncFaultWithOneEventCommand(101, Guid.NewGuid());
            await GridNode.PrepareCommand(syncCommand)
                          .Expect<AggregateChangedEventNotification>(e => e.AggregateId == syncCommand.AggregateId)
                          .Execute()
                          .ShouldThrow<SampleAggregateException>();
        }

        [Then]
        public async Task When_aggregate_throws_fault_it_is_handled_without_implicit_registration()
        {
            //will throw exception in aggregate and in message handler
            var syncCommand = new AsyncFaultWithOneEventCommand(50, Guid.NewGuid());
            var res = await GridNode.PrepareCommand(syncCommand)
                                    .Expect<AggregateChangedEventNotification>()
                                    .Or<IFault<SampleAggregateChangedEvent>>()
                                    .Execute(false);

            Assert.NotNull(res.Message<IFault<AsyncFaultWithOneEventCommand>>());
        }


        [Then]
        public async Task When_fault_was_received_and_failOnFaults_is_set_results_raised_an_error()
        {
            var syncCommand = new AsyncFaultWithOneEventCommand(100, Guid.NewGuid());
            await GridNode.PrepareCommand(syncCommand)
                          .Expect<AggregateChangedEventNotification>()
                          .Execute()
                          .ShouldThrow<SampleAggregateException>();
        }
    }
}