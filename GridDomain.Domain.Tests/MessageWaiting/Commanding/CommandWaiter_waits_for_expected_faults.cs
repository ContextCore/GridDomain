using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.Logging;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.CommandsExecution;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using GridDomain.Tests.SampleDomain.ProjectionBuilders;
using NUnit.Framework;

namespace GridDomain.Tests.MessageWaiting.Commanding
{
    [TestFixture]
    public class CommandWaiter_waits_for_expected_faults : SampleDomainCommandExecutionTests
    {
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
        public void When_expected_fault_received_it_contains_error()
        {
            var syncCommand = new LongOperationCommand(100, Guid.NewGuid());
            var res = GridNode.NewCommandWaiter(Timeout, false)
                                .Expect<AggregateChangedEventNotification>(e => e.AggregateId == syncCommand.AggregateId)
                                .Or<IFault<SampleAggregateChangedEvent>>(f => f.Message.SourceId == syncCommand.AggregateId && 
                                                                                  (f.Processor == typeof(EvenFaultyMessageHandler) || 
                                                                                   f.Processor == typeof(OddFaultyMessageHandler)))
                              .Create()
                              .Execute(syncCommand)
                              .Result;

            Assert.IsInstanceOf<MessageHandleException>(res.Message<IFault<SampleAggregateChangedEvent>>()?.Exception);
        }

        [Then]
        public void When_expecting_generic_fault_without_processor_received_fault_contains_error()
        {
            var syncCommand = new LongOperationCommand(100, Guid.NewGuid());
            var res = GridNode.NewCommandWaiter(Timeout,false)
                                .Expect<AggregateChangedEventNotification>(e => e.AggregateId == syncCommand.AggregateId)
                                .Or<IFault>(f => (f.Message as DomainEvent)?.SourceId == syncCommand.AggregateId)
                              .Create()
                              .Execute(syncCommand)
                              .Result;

            Assert.IsInstanceOf<MessageHandleException>(res.Message<IFault<SampleAggregateChangedEvent>>()?.Exception);
        }

        [Then]
        public void When_does_not_expect_fault_and_it_accures_wait_times_out()
        {
            var syncCommand = new LongOperationCommand(100, Guid.NewGuid());
            try
            {
                GridNode.NewCommandWaiter(Timeout)
                           .Expect<AggregateChangedEventNotification>(e => e.AggregateId == syncCommand.AggregateId)
                         .Create()
                         .Execute(syncCommand)
                         .Wait();
            }
            catch (AggregateException ex)
            {
                Assert.IsInstanceOf<TimeoutException>(ex.UnwrapSingle(), ex.InnerException.ToPropsString());
            }
        }


        [Then]
        public void When_expected_optional_fault_does_not_occur_wait_is_successfull()
        {
            var syncCommand = new LongOperationCommand(101, Guid.NewGuid());
            var res = GridNode.NewCommandWaiter(Timeout)
                                .Expect<AggregateChangedEventNotification>(e => e.AggregateId == syncCommand.AggregateId)
                                .Or<IFault>(f => (f.Message as DomainEvent)?.SourceId == syncCommand.AggregateId)
                              .Create()
                              .Execute(syncCommand)
                              .Result;

            var evt = res.Message<AggregateChangedEventNotification>();
            Assert.AreEqual(syncCommand.AggregateId, evt.AggregateId);
        }

        [Then]
        public async Task When_aggregate_throws_fault_it_is_handled_without_implicit_registration()
        {
            //will throw exception in aggregate and in message handler
            var syncCommand = new AsyncFaultWithOneEventCommand(50, Guid.NewGuid());

            var res = await GridNode.NewCommandWaiter(Timeout, false)
                                    .Expect<AggregateChangedEventNotification>()
                                    .Or<IFault<SampleAggregateChangedEvent>>()
                                    .Create()
                                    .Execute(syncCommand);

            Assert.NotNull(res.Message<IFault<AsyncFaultWithOneEventCommand>>());
        }


        [Then]
        public async Task When_fault_was_received_and_failOnFaults_is_set_results_raised_an_error()
        {
            var syncCommand = new AsyncFaultWithOneEventCommand(500, Guid.NewGuid());
            await GridNode.NewCommandWaiter(Timeout)
                          .Expect<AggregateChangedEventNotification>()
                          .Create()
                          .Execute(syncCommand)
                          .ShouldThrow<SampleAggregateException>();
        }
    }
}