using System;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Logging;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using GridDomain.Tests.SampleDomain.ProjectionBuilders;
using NUnit.Framework;

namespace GridDomain.Tests.CommandsExecution.ExpectedMessages
{
    [TestFixture]
    public class When_SyncExecute_until_projection_fault : SampleDomainCommandExecutionTests
    {
        public When_SyncExecute_until_projection_fault() : base(true)
        {
        }

        //protected override bool CreateNodeOnEachTest { get; } = true;

        public When_SyncExecute_until_projection_fault(bool inMemory = true) : base(inMemory)
        {
        }

        protected override IMessageRouteMap CreateMap()
        {
            var faultyHandlerMap =
                new CustomRouteMap(
                    r => r.RegisterHandler<SampleAggregateChangedEvent, OddFaultyMessageHandler>(e => e.SourceId),
                    r => r.RegisterHandler<SampleAggregateCreatedEvent, CreateProjectionBuilder>(e => e.SourceId),
                    r => r.RegisterAggregate(SampleAggregatesCommandHandler.Descriptor));

            return new CompositeRouteMap(faultyHandlerMap);
        }


        [Then]
        public void SyncExecute_with_projection_fault_expecting_fault_from_different_sources_delivers_error_to_caller_from_registered_source()
        {
            var syncCommand = new LongOperationCommand(100, Guid.NewGuid());
            var expectedFault = Expect.Fault<SampleAggregateChangedEvent>(e => e.SourceId,
                                                                          syncCommand.AggregateId,
                                                                          typeof(EvenFaultyMessageHandler),
                                                                          typeof(OddFaultyMessageHandler));

            var expectedMessage = Expect.Message<AggregateChangedEventNotification>(e => e.AggregateId,
                                                                                    syncCommand.AggregateId);

            AssertEx.ThrowsInner<MessageHandleException>(
                ()=>
                    GridNode.Execute(new CommandPlan(syncCommand,expectedFault, expectedMessage))
                            .Wait()
                );
        }

        [Then]
        public void SyncExecute_with_projection_fault_expecting_fault_wihout_source_type_delivers_error_to_caller_from_any_source()
        {
            var syncCommand = new LongOperationCommand(100, Guid.NewGuid());
            var expectedFault = Expect.Fault<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId);
            var expectedMessage = Expect.Message<AggregateChangedEventNotification>(e => e.AggregateId,
                syncCommand.AggregateId);

            AssertEx.ThrowsInner<MessageHandleException>(() =>
                GridNode.Execute(new CommandPlan(syncCommand, expectedFault, expectedMessage)).Wait());
        }

        [Then]
        public void SyncExecute_with_projection_fault_without_expectation_times_out()
        {
            var syncCommand = new LongOperationCommand(100, Guid.NewGuid());
            var expectedMessage = Expect.Message<AggregateChangedEventNotification>(e => e.AggregateId,syncCommand.AggregateId);

            var plan = new CommandPlan(syncCommand, TimeSpan.FromMilliseconds(50),expectedMessage);

            AssertEx.ThrowsInner<TimeoutException>(() => GridNode.Execute(plan).Wait());
        }

        [Then]
        public void SyncExecute_with_projection_fault_with_expectation_in_plan_deliver_exception_to_caller()
        {
            var syncCommand = new LongOperationCommand(100, Guid.NewGuid());
            var expectedFault = Expect.Fault<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId,
                typeof(OddFaultyMessageHandler));
            var expectedMessage = Expect.Message<AggregateChangedEventNotification>(e => e.AggregateId,
                syncCommand.AggregateId);
            var plan = new CommandPlan<AggregateChangedEventNotification>(syncCommand, expectedMessage, expectedFault);


            AssertEx.ThrowsInner<MessageHandleException>(() => GridNode.Execute(plan).Wait());
        }

        [Then]
        public void SyncExecute_with_projection_success_with_expectation_in_plan_deliver_message_to_caller()
        {
            var syncCommand = new LongOperationCommand(101, Guid.NewGuid());
            var expectedFault = Expect.Fault<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId,
                typeof(OddFaultyMessageHandler));
            var expectedMessage = Expect.Message<AggregateChangedEventNotification>(e => e.AggregateId,
                syncCommand.AggregateId);
            var plan = new CommandPlan<AggregateChangedEventNotification>(syncCommand, expectedMessage, expectedFault);


            var evt = GridNode.Execute<AggregateChangedEventNotification>(plan).Result;
            Assert.AreEqual(syncCommand.AggregateId, evt.AggregateId);
        }


        [Then]
        public void SyncExecute_with_projection_fault_with_correct_typed_fault_expectation_deliver_error_to_caller()
        {
            //will throw exception in aggregate and in message handler
            var syncCommand = new LongOperationCommand(500, Guid.NewGuid());
            var expectedFault = Expect.Fault<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId,
                typeof(OddFaultyMessageHandler));
            var expectedMessage = Expect.Message<AggregateChangedEventNotification>(e => e.AggregateId,
                syncCommand.AggregateId);

            AssertEx.ThrowsInner<MessageHandleException>(() =>
            {
                GridNode.Execute<object>(syncCommand, expectedFault, expectedMessage).Wait();
            });
        }


        [Then]
        public void SyncExecute_with_projection_fault_with_incorrect_typed_fault_times_out()
        {
            var syncCommand = new LongOperationCommand(100, Guid.NewGuid());
            var expectedFault = Expect.Fault<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId,
                typeof(string));
            var expectedMessage = Expect.Message<AggregateChangedEventNotification>(e => e.AggregateId,
                syncCommand.AggregateId);

            AssertEx.ThrowsInner<TimeoutException>(() =>
            {
                GridNode.Execute<AggregateChangedEventNotification>(syncCommand, expectedFault, expectedMessage).Wait();
            });
        }

        [Then]
        public void When_aggregate_throws_fault_it_is_handled_without_implicit_registration()
        {
            //will throw exception in aggregate and in message handler
            var syncCommand = new AsyncFaultWithOneEventCommand(500, Guid.NewGuid());
            var expectedFault = Expect.Fault<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId,
                typeof(OddFaultyMessageHandler));
            var expectedMessage = Expect.Message<AggregateChangedEventNotification>(e => e.AggregateId,
                                                                                    syncCommand.AggregateId);

            AssertEx.ThrowsInner<SampleAggregateException>(() =>
            {
                GridNode.Execute<AggregateChangedEventNotification>(syncCommand, expectedFault, expectedMessage).Wait();
            });
        }

        [Then]
        public void When_one_of_two_aggregate_throws_fault_not_received_expected_messages_are_ignored()
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
                GridNode.Execute(new CommandPlan(syncCommand, messages)).Wait();
                Assert.Fail("Wait ended after one of two notifications");
            }
            catch (AggregateException ex)
            {
                var exception = ex.InnerException;

                if (exception is SampleAggregateException) Assert.Pass("Got exception from create message handler");
                if (exception is MessageHandleException) Assert.Pass("Got exception from change message handler");
                Assert.Fail("Unknown exception type");
            }
        }
    }
}