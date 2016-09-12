using System;
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
        public void SyncExecute_until_projection()
        {
            var syncCommand = new LongOperationCommand(101, Guid.NewGuid());
            var expectedMessage = ExpectedMessage.Once<AggregateChangedEventNotification>(e => e.AggregateId, syncCommand.AggregateId);

            GridNode.Execute(syncCommand, expectedMessage).Wait();

            var aggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.AreEqual(syncCommand.Parameter.ToString(), aggregate.Value);
        }

        [Then]
        public void SyncExecute_waiting_for_projection_notification_with_fault_in_projection()
        {
            var syncCommand = new LongOperationCommand(100, Guid.NewGuid());
            var expectedMessage = ExpectedMessage.Once<AggregateChangedEventNotification>(e => e.AggregateId, syncCommand.AggregateId);

            try
            {
                var result = GridNode.Execute(syncCommand, expectedMessage).Result;
            }
            catch (Exception ex)
            {
                var exception = ex.InnerException;
                Assert.IsInstanceOf<OddFaultyMessageHandler.MessageHandleException>(exception);
            }
        }

        [Then]
        public void SyncExecute_until_projection_fault_then_exception_from_handler_is_delivered_to_caller()
        {
            var syncCommand = new LongOperationCommand(100, Guid.NewGuid());
            var expectedMessage = ExpectedMessage.Once<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId);

            try
            {
                var result = GridNode.Execute(syncCommand, expectedMessage).Result;
            }
            catch (Exception ex)
            {
                var exception = ex.InnerException;
                Assert.IsInstanceOf<OddFaultyMessageHandler.MessageHandleException>(exception);
            }
        }


        [Then]
        public void Waiting_for_several_events_stops_on_first_event_fault()
        {
            //will throw exeption in aggregate and in message handler
            //waiting for event from message handlers
            var syncCommand = new AsyncFaultWithOneEventCommand(100,Guid.NewGuid());
            var expectedMessage = ExpectedMessage.Once<AggregateChangedEventNotification>(e => e.AggregateId, syncCommand.AggregateId);

            try
            {
                GridNode.Execute(syncCommand, expectedMessage).Wait();
            }
            catch (Exception ex)
            {
                var exception = ex.InnerException;
                Assert.IsInstanceOf<OddFaultyMessageHandler.MessageHandleException>(exception);
            }
        }
    }
}