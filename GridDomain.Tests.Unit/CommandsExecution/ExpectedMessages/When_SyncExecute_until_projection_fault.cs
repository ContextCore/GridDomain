using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Tests.Unit.SampleDomain;
using GridDomain.Tests.Unit.SampleDomain.Commands;
using GridDomain.Tests.Unit.SampleDomain.Events;
using GridDomain.Tests.Unit.SampleDomain.ProjectionBuilders;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.CommandsExecution.ExpectedMessages
{
    [TestFixture]
    public class When_SyncExecute_until_projection_fault : SampleDomainCommandExecutionTests
    {
        public When_SyncExecute_until_projection_fault() : base(true)
        {
        }


        public When_SyncExecute_until_projection_fault(bool inMemory = true) : base(inMemory)
        {
        }


       // protected override bool CreateNodeOnEachTest { get; } = true;

        protected override IMessageRouteMap CreateMap()
        {
            var faultyHandlerMap =
                new CustomRouteMap(
                    r => r.RegisterHandler<SampleAggregateChangedEvent, OddFaultyMessageHandler>(e => e.SourceId),
                    r => r.RegisterHandler<SampleAggregateCreatedEvent, FaultyCreateProjectionBuilder>(e => e.SourceId),
                    r => r.RegisterAggregate(SampleAggregatesCommandHandler.Descriptor));

            return new CompositeRouteMap(faultyHandlerMap);
        }


        [Then]
        public async Task SWhen_execute_waiting_without_timeout_default_timeout_is_used()
        {
            var syncCommand = new LongOperationCommand(100, Guid.NewGuid());
            var expectedMessage = Expect.Message<AggregateChangedEventNotification>(e => e.AggregateId, syncCommand.AggregateId);

            var plan = new CommandPlan(syncCommand, TimeSpan.FromMilliseconds(50), expectedMessage);

            await GridNode.Execute(plan)
                          .ShouldThrow<TimeoutException>();
        }

        [Then]

        public async Task SyncExecute_with_projection_fault_expecting_fault_from_different_sources_delivers_error_to_caller_from_registered_source()
        {
            var syncCommand = new LongOperationCommand(100, Guid.NewGuid());
            var expectedFault = Expect.Fault<SampleAggregateChangedEvent>(e => e.SourceId,
                                                                          syncCommand.AggregateId,
                                                                          typeof(EvenFaultyMessageHandler),
                                                                          typeof(OddFaultyMessageHandler));

            var expectedMessage = Expect.Message<AggregateChangedEventNotification>(e => e.AggregateId,
                                                                                    syncCommand.AggregateId);

            await GridNode.Execute(new CommandPlan(syncCommand,expectedFault, expectedMessage))
                .ShouldThrow<MessageHandleException>();
        }

        [Then]
        public async Task SyncExecute_with_projection_fault_expecting_fault_wihout_source_type_delivers_error_to_caller_from_any_source()
        {
            var syncCommand = new LongOperationCommand(100, Guid.NewGuid());
            var expectedFault = Expect.Fault<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId);
            var expectedMessage = Expect.Message<AggregateChangedEventNotification>(e => e.AggregateId,
                                                                                    syncCommand.AggregateId);

            await GridNode.Execute(new CommandPlan(syncCommand, expectedFault, expectedMessage))
                  .ShouldThrow<MessageHandleException>();
        }

    

        [Then]
        public async Task SyncExecute_with_projection_fault_with_expectation_in_plan_deliver_exception_to_caller()
        {
            var syncCommand = new LongOperationCommand(100, Guid.NewGuid());
            var expectedFault = Expect.Fault<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId,
                                                                                            typeof(OddFaultyMessageHandler));
            var expectedMessage = Expect.Message<AggregateChangedEventNotification>(e => e.AggregateId,
                                                                                    syncCommand.AggregateId);

            var plan = new CommandPlan<AggregateChangedEventNotification>(syncCommand, expectedMessage, expectedFault);

            await GridNode.Execute(plan)
                          .ShouldThrow<MessageHandleException>();
        }

        [Then]
        public async Task SyncExecute_with_projection_success_with_expectation_in_plan_deliver_message_to_caller()
        {
            var syncCommand = new LongOperationCommand(101, Guid.NewGuid());
            var expectedFault = Expect.Fault<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId,
                typeof(OddFaultyMessageHandler));
            var expectedMessage = Expect.Message<AggregateChangedEventNotification>(e => e.AggregateId,
                syncCommand.AggregateId);
            var plan = new CommandPlan<AggregateChangedEventNotification>(syncCommand, expectedMessage, expectedFault);

            var evt = await GridNode.Execute(plan);
            Assert.AreEqual(syncCommand.AggregateId, evt.AggregateId);
        }


        [Then]
        public async Task SyncExecute_with_projection_fault_with_correct_typed_fault_expectation_deliver_error_to_caller()
        {
            //will throw exception in aggregate and in message handler
            var syncCommand = new LongOperationCommand(500, Guid.NewGuid());
            var expectedFault = Expect.Fault<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId,
                typeof(OddFaultyMessageHandler));
            var expectedMessage = Expect.Message<AggregateChangedEventNotification>(e => e.AggregateId,
                syncCommand.AggregateId);

            await GridNode.Execute<object>(syncCommand, expectedFault, expectedMessage)
                          .ShouldThrow<MessageHandleException>();
        }


        [Then]

        public async Task SyncExecute_with_projection_fault_with_incorrect_typed_fault_times_out()
        {
            var syncCommand = new LongOperationCommand(100, Guid.NewGuid());
            var expectedFault = Expect.Fault<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId,
                                                                           typeof(string));
            var expectedMessage = Expect.Message<AggregateChangedEventNotification>(e => e.AggregateId,
                                                                                syncCommand.AggregateId);

            await GridNode.Execute<AggregateChangedEventNotification>(syncCommand, expectedFault, expectedMessage)
                          .ShouldThrow<TimeoutException>();
        }

        [Then]
        public async Task When_aggregate_throws_fault_it_is_handled_without_implicit_registration()
        {
            //will throw exception in aggregate and in message handler
            var syncCommand = new AsyncFaultWithOneEventCommand(500, Guid.NewGuid());
            var expectedFault = Expect.Fault<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId,
                typeof(OddFaultyMessageHandler));
            var expectedMessage = Expect.Message<AggregateChangedEventNotification>(e => e.AggregateId,
                                                                                    syncCommand.AggregateId);

            await GridNode.Execute<AggregateChangedEventNotification>(syncCommand, expectedFault, expectedMessage)
                          .ShouldThrow<SampleAggregateException>();
        }

        [Then]
        public async Task When_one_of_two_aggregate_throws_fault_not_received_expected_messages_are_ignored()
        {
            var syncCommand = new CreateAndChangeSampleAggregateCommand(100, Guid.NewGuid());
            var messages = new ExpectedMessage[]
            {
                Expect.Fault<SampleAggregateCreatedEvent>(e => e.SourceId, syncCommand.AggregateId),
                Expect.Fault<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId),
                Expect.Message<AggregateChangedEventNotification>(e => e.AggregateId, syncCommand.AggregateId),
                Expect.Message<AggregateCreatedEventNotification>(e => e.AggregateId, syncCommand.AggregateId)
            };

            try
            {
                await GridNode.Execute(new CommandPlan(syncCommand, messages));
                Assert.Fail("Wait ended after one of two notifications");
            }
            catch (AggregateException ex)
            {
                var exception = ex.UnwrapSingle();

                if (exception is SampleAggregateException) Assert.Pass("Got exception from create message handler");
                if (exception is MessageHandleException) Assert.Pass("Got exception from change message handler");
                Assert.Fail($"Unknown exception type: {exception.GetType()}");
            }
        }
    }
}