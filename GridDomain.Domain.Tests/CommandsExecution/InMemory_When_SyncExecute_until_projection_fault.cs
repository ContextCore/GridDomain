using System;
using System.Threading;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using GridDomain.Tests.SampleDomain.ProjectionBuilders;
using NUnit.Framework;

namespace GridDomain.Tests.CommandsExecution
{
    [TestFixture]
    public class InMemory_When_SyncExecute_until_projection_fault : SampleDomainCommandExecutionTests
    {

        public InMemory_When_SyncExecute_until_projection_fault() : base(true)
        {

        }
        public InMemory_When_SyncExecute_until_projection_fault(bool inMemory = true) : base(inMemory)
        {

        }

        protected override IMessageRouteMap CreateMap()
        {
            var faultyHandlerMap =
                new CustomRouteMap(
                    r => r.RegisterHandler<SampleAggregateChangedEvent, OddFaultyMessageHandler>(e => e.SourceId),
                    r => r.RegisterAggregate(SampleAggregatesCommandHandler.Descriptor));

            return new CompositeRouteMap(faultyHandlerMap);
        }



        [Then]
        public void SyncExecute_with_projection_fault_expecting_fault_wihout_source_type_delivers_error_to_caller_from_any_source()
        {
            var syncCommand = new LongOperationCommand(100, Guid.NewGuid());
            var expectedFault = ExpectedMessage.Fault<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId);
            var expectedMessage = ExpectedMessage.Once<AggregateChangedEventNotification>(e => e.AggregateId, syncCommand.AggregateId);

            try
            {
                var result = GridNode.Execute(syncCommand,new ExpectedMessage[] { expectedFault, expectedMessage},TimeSpan.FromSeconds(100)).Result;
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<OddFaultyMessageHandler.MessageHandleException>(ex.InnerException);
            }
        }

        [Then]
        public void SyncExecute_with_projection_fault_without_expectation_times_out()
        {
            var syncCommand = new LongOperationCommand(100, Guid.NewGuid());
            var expectedMessage = ExpectedMessage.Once<AggregateChangedEventNotification>(e => e.AggregateId, syncCommand.AggregateId);
            var plan = new CommandPlan(syncCommand,expectedMessage);
            try
            {
                GridNode.Execute(plan, TimeSpan.FromMilliseconds(500));
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<TimeoutException>(ex);
            }
        }

        [Then]
        public void SyncExecute_with_projection_fault_with_correct_typed_fault_expectation_deliver_error_to_caller()
        {
            //will throw exception in aggregate and in message handler
            var syncCommand = new LongOperationCommand(500, Guid.NewGuid());
            var expectedFault = ExpectedMessage.Fault<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId, typeof(OddFaultyMessageHandler));
            var expectedMessage = ExpectedMessage.Once<AggregateChangedEventNotification>(e => e.AggregateId, syncCommand.AggregateId);

            try
            {
                GridNode.Execute<object>(syncCommand, expectedFault, expectedMessage).Wait();
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<OddFaultyMessageHandler.MessageHandleException>(ex.InnerException);
            }
        }


        [Then]
        public void SyncExecute_with_projection_fault_with_incorrect_typed_fault_times_out()
        {
            var syncCommand = new LongOperationCommand(100, Guid.NewGuid());
            var expectedFault = ExpectedMessage.Fault<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId, typeof(string));
            var expectedMessage = ExpectedMessage.Once<AggregateChangedEventNotification>(e => e.AggregateId, syncCommand.AggregateId);

            try
            {
                GridNode.Execute<AggregateChangedEventNotification>(syncCommand, expectedFault, expectedMessage).Wait();
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<TimeoutException>(ex.InnerException);
            }
        }

        [Then]
        public void When_aggregate_throws_fault_it_is_handled_without_implicit_registration()
        {
            //will throw exception in aggregate and in message handler
            var syncCommand = new AsyncFaultWithOneEventCommand(500,Guid.NewGuid());
            var expectedFault = ExpectedMessage.Fault<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId, typeof(OddFaultyMessageHandler));
            var expectedMessage = ExpectedMessage.Once<AggregateChangedEventNotification>(e => e.AggregateId, syncCommand.AggregateId);

            try
            {
                GridNode.Execute<AggregateChangedEventNotification>(syncCommand, expectedFault, expectedMessage).Wait();
            }
            catch (Exception ex)
            {
                var exception = ex.InnerException;
                Assert.IsInstanceOf<SampleAggregateException>(exception);
            }
        }
    }
}